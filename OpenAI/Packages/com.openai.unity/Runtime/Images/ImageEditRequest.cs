// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace OpenAI.Images
{
    public sealed class ImageEditRequest : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </param>
        /// <param name="maskPath">
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </param>
        /// <param name="prompt">
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </param>
        /// <param name="numberOfResults">
        /// The number of images to generate. Must be between 1 and 10.
        /// </param>
        /// <param name="size">
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        public ImageEditRequest(string imagePath, string maskPath, string prompt, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null)
            : this(
                File.OpenRead(imagePath),
                Path.GetFileName(imagePath),
                string.IsNullOrWhiteSpace(maskPath) ? null : File.OpenRead(maskPath),
                string.IsNullOrWhiteSpace(maskPath) ? null : Path.GetFileName(maskPath),
                prompt,
                numberOfResults,
                size,
                user)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="texture">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </param>
        /// <param name="mask">
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </param>
        /// <param name="prompt">
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </param>
        /// <param name="numberOfResults">
        /// The number of images to generate. Must be between 1 and 10.
        /// </param>
        /// <param name="size">
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        public ImageEditRequest(Texture2D texture, Texture2D mask, string prompt, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null)
            : this(
                new MemoryStream(texture.EncodeToPNG()),
                $"{texture.name}.png",
                mask != null ? new MemoryStream(mask.EncodeToPNG()) : null,
                mask != null ? $"{mask.name}.png" : null,
                prompt,
                numberOfResults,
                size,
                user)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="image">
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </param>
        /// <param name="imageName">Name of the image file.</param>
        /// <param name="mask">
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </param>
        /// <param name="maskName">Name of the mask file.</param>
        /// <param name="prompt">
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </param>
        /// <param name="numberOfResults">
        /// The number of images to generate. Must be between 1 and 10.
        /// </param>
        /// <param name="size">
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </param>
        /// <param name="user">
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </param>
        public ImageEditRequest(Stream image, string imageName, Stream mask, string maskName, string prompt, int numberOfResults = 1, ImageSize size = ImageSize.Large, string user = null)
        {
            Image = image;

            if (string.IsNullOrWhiteSpace(imageName))
            {
                imageName = "image.png";
            }

            ImageName = imageName;

            if (mask != null)
            {
                Mask = mask;

                if (string.IsNullOrWhiteSpace(maskName))
                {
                    maskName = "mask.png";
                }

                MaskName = maskName;
            }

            if (prompt.Length > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
            }

            Prompt = prompt;

            if (numberOfResults is > 10 or < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            }

            Number = numberOfResults;

            Size = size switch
            {
                ImageSize.Small => "256x256",
                ImageSize.Medium => "512x512",
                ImageSize.Large => "1024x1024",
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };

            User = user;
        }

        ~ImageEditRequest() => Dispose(false);

        /// <summary>
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </summary>
        public Stream Image { get; }

        public string ImageName { get; }

        /// <summary>
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </summary>
        public Stream Mask { get; }

        public string MaskName { get; }

        /// <summary>
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </summary>
        public string Prompt { get; }

        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        public string Size { get; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        public string User { get; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Close();
                Image?.Dispose();
                Mask?.Dispose();
                Mask?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
