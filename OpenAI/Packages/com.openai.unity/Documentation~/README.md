# OpenAI

[![Discord](https://img.shields.io/discord/855294214065487932.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/xQgMW9ufN4)
[![openupm](https://img.shields.io/npm/v/com.openai.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.openai.unity/)

Based on [OpenAI-DotNet](https://github.com/RageAgainstThePixel/OpenAI-DotNet)

A [OpenAI](https://openai.com/) package for the [Unity](https://unity.com/) Game Engine to use GPT-3 and Dall-E though their RESTful API (currently in beta).  Independently developed, this is not an official library and I am not affiliated with OpenAI.  An OpenAI API account is required.

> This repository is available to transfer to the OpenAI organization if they so choose to accept it.

## Installing

Requires Unity 2021.3 LTS or higher.

The recommended installation method is though the unity package manager and [OpenUPM](https://openupm.com/packages/com.openai.unity).

### Via Unity Package Manager and OpenUPM

- Open your Unity project settings
- Add the OpenUPM package registry:
  - `Name: OpenUPM`
  - `URL: https://package.openupm.com`
  - `Scope(s):`
    - `com.openai`
    - `com.utilities`

![scoped-registries](images/package-manager-scopes.png)

- Open the Unity Package Manager window
- Change the Registry from Unity to `My Registries`
- Add the `OpenAI` package

### Via Unity Package Manager and Git url

- Open your Unity Package Manager
- Add package from git url: `https://github.com/RageAgainstThePixel/com.openai.unity.git#upm`
  > Note: this repo has dependencies on other repositories! You are responsible for adding these on your own.
  - [com.utilities.rest](https://github.com/RageAgainstThePixel/com.utilities.rest)
  - [com.utilities.async](https://github.com/RageAgainstThePixel/com.utilities.async)

---

## Documentation

### Table of Contents

- [Authentication](#authentication)
- [Models](#models)
  - [List Models](#list-models)
  - [Retrieve Models](#retrieve-model)
  - [Delete Fine Tuned Model](#delete-fine-tuned-model)
- [Completions](#completions)
  - [Streaming](#streaming)
- [Edits](#edits)
  - [Create Edit](#create-edit)
- [Embeddings](#embeddings)
  - [Create Embedding](#create-embeddings)
- [Images](#images)
  - [Create Image](#create-image)
  - [Edit Image](#edit-image)
  - [Create Image Variation](#create-image-variation)
- [Files](#files)
  - [List Files](#list-files)
  - [Upload File](#upload-file)
  - [Delete File](#delete-file)
  - [Retrieve File Info](#retrieve-file-info)
  - [Download File Content](#download-file-content)
- [Fine Tuning](#fine-tuning)
  - [Create Fine Tune Job](#create-fine-tune-job)
  - [List Fine Tune Jobs](#list-fine-tune-jobs)
  - [Retrieve Fine Tune Job Info](#retrieve-fine-tune-job-info)
  - [Cancel Fine Tune Job](#cancel-fine-tune-job)
  - [List Fine Tune Events](#list-fine-tune-events)
  - [Stream Fine Tune Events](#stream-fine-tune-events)
- [Moderations](#moderations)
  - [Create Moderation](#create-moderation)

### Authentication

There are 4 ways to provide your API keys, in order of precedence:

1. [Pass keys directly with constructor](#pass-keys-directly-with-constructor)
2. [Unity Scriptable Object](#unity-scriptable-object)
3. [Load key from configuration file](#load-key-from-configuration-file)
4. [Use System Environment Variables](#use-system-environment-variables)

#### Pass keys directly with constructor

```csharp
var api = new OpenAIClient("sk-apiKey");
```

Or create a `OpenAIAuthentication` object manually

```csharp
var api = new OpenAIClient(new OpenAIAuthentication("sk-apiKey", "org-yourOrganizationId"));
```

#### Unity Scriptable Object

You can save the key directly into a scriptable object that is located in the `Assets/Resources` folder.

You can create a new one by using the context menu of the project pane and creating a new `OpenAIConfigurationSettings` scriptable object.

![Create new OpenAIConfigurationSettings](images/create-scriptable-object.png)

#### Load key from configuration file

Attempts to load api keys from a configuration file, by default `.openai` in the current directory, optionally traversing up the directory tree or in the user's home directory.

To create a configuration file, create a new text file named `.openai` and containing the line:

> Organization entry is optional.

##### Json format

```json
{
  "apiKey": "sk-aaaabbbbbccccddddd",
  "organization": "org-yourOrganizationId"
}
```

##### Deprecated format

```shell
OPENAI_KEY=sk-aaaabbbbbccccddddd
ORGANIZATION=org-yourOrganizationId
```

You can also load the file directly with known path by calling a static method in Authentication:

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory("your/path/to/.openai"));;
```

#### Use System Environment Variables

Use your system's environment variables specify an api key and organization to use.

- Use `OPENAI_API_KEY` for your api key.
- Use `OPENAI_ORGANIZATION_ID` to specify an organization.

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
```

### [Models](https://beta.openai.com/docs/api-reference/models)

List and describe the various models available in the API. You can refer to the [Models documentation](https://beta.openai.com/docs/models) to understand what models are available and the differences between them.

The Models API is accessed via `OpenAIClient.ModelsEndpoint`

#### [List models](https://beta.openai.com/docs/api-reference/models/list)

Lists the currently available models, and provides basic information about each one such as the owner and availability.

```csharp
var api = new OpenAIClient();
var models = await api.ModelsEndpoint.GetModelsAsync();

foreach (var model in models)
{
    Debug.Log(model.ToString());
}
```

#### [Retrieve model](https://beta.openai.com/docs/api-reference/models/retrieve)

Retrieves a model instance, providing basic information about the model such as the owner and permissioning.

```csharp
var api = new OpenAIClient();
var model = await api.ModelsEndpoint.GetModelDetailsAsync("text-davinci-003");
Debug.Log(model.ToString());
```

#### [Delete Fine Tuned Model](https://beta.openai.com/docs/api-reference/fine-tunes/delete-model)

Delete a fine-tuned model. You must have the Owner role in your organization.

```csharp
var api = new OpenAIClient();
var result = await api.ModelsEndpoint.DeleteFineTuneModelAsync("your-fine-tuned-model");
Assert.IsTrue(result);
```

### [Completions](https://beta.openai.com/docs/api-reference/completions)

Given a prompt, the model will return one or more predicted completions, and can also return the probabilities of alternative tokens at each position.

```csharp
var api = new OpenAIClient();
var result = await api.CompletionsEndpoint.CreateCompletionAsync("One Two Three One Two", temperature: 0.1, model: Model.Davinci);
Debug.Log(result);
```

> To get the `CompletionResult` (which is mostly metadata), use its implicit string operator to get the text if all you want is the completion choice.

#### Streaming

Streaming allows you to get results are they are generated, which can help your application feel more responsive, especially on slow models like Davinci.

```csharp
var api = new OpenAIClient();

await api.CompletionsEndpoint.StreamCompletionAsync(result =>
{
    foreach (var choice in result.Completions)
    {
        Debug.Log(choice);
    }
}, "My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", maxTokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci);
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
await foreach (var token in api.CompletionsEndpoint.StreamCompletionEnumerableAsync("My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", maxTokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci))
{
  Debug.Log(token);
}
```

### [Edits](https://beta.openai.com/docs/api-reference/edits)

Given a prompt and an instruction, the model will return an edited version of the prompt.

The Edits API is accessed via `OpenAIClient.EditsEndpoint`

#### [Create Edit](https://beta.openai.com/docs/api-reference/edits/create)

Creates a new edit for the provided input, instruction, and parameters using the provided input and instruction.

```csharp
var api = new OpenAIClient();
var request = new EditRequest("What day of the wek is it?", "Fix the spelling mistakes");
var result = await api.EditsEndpoint.CreateEditAsync(request);
Debug.Log(result);
```

### [Embeddings](https://beta.openai.com/docs/api-reference/embeddings)

Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.

Related guide: [Embeddings](https://beta.openai.com/docs/guides/embeddings)

The Edits API is accessed via `OpenAIClient.EmbeddingsEndpoint`

#### [Create Embeddings](https://beta.openai.com/docs/api-reference/embeddings/create)

Creates an embedding vector representing the input text.

```csharp
var api = new OpenAIClient();
var result = await api.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...");
Debug.Log(result);
```

### [Images](https://beta.openai.com/docs/api-reference/images)

Given a prompt and/or an input image, the model will generate a new image.

The Images API is accessed via `OpenAIClient.ImagesEndpoint`

#### [Create Image](https://beta.openai.com/docs/api-reference/images/create)

Creates an image given a prompt.

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.GenerateImageAsync("A house riding a velociraptor", 1, ImageSize.Small);

foreach (var (path, texture) in results)
{
    Debug.Log(path);
    // path == file://path/to/image.png
    Assert.IsNotNull(texture);
    // texture == The preloaded Texture2D
}
```

#### [Edit Image](https://beta.openai.com/docs/api-reference/images/create-edit)

Creates an edited or extended image given an original image and a prompt.

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.CreateImageEditAsync(Path.GetFullPath(imageAssetPath), Path.GetFullPath(maskAssetPath), "A sunlit indoor lounge area with a pool containing a flamingo", 1, ImageSize.Small);

foreach (var (path, texture) in results)
{
    Debug.Log(path);
    // path == file://path/to/image.png
    Assert.IsNotNull(texture);
    // texture == The preloaded Texture2D
}
```

#### [Create Image Variation](https://beta.openai.com/docs/api-reference/images/create-variation)

Creates a variation of a given image.

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.CreateImageVariationAsync(Path.GetFullPath(imageAssetPath), 1, ImageSize.Small);

foreach (var (path, texture) in results)
{
    Debug.Log(path);
    // path == file://path/to/image.png
    Assert.IsNotNull(texture);
    // texture == The preloaded Texture2D
}
```

### [Files](https://beta.openai.com/docs/api-reference/files)

Files are used to upload documents that can be used with features like [Fine-tuning](#fine-tuning).

The Files API is accessed via `OpenAIClient.FilesEndpoint`

#### [List Files](https://beta.openai.com/docs/api-reference/files/list)

Returns a list of files that belong to the user's organization.

```csharp
var api = new OpenAIClient();
var files = await api.FilesEndpoint.ListFilesAsync();

foreach (var file in files)
{
    Debug.Log($"{file.Id} -> {file.Object}: {file.FileName} | {file.Size} bytes");
}
```

#### [Upload File](https://beta.openai.com/docs/api-reference/files/upload)

Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.

```csharp
var api = new OpenAIClient();
var fileData = await api.FilesEndpoint.UploadFileAsync("path/to/your/file.jsonl", "fine-tune");
Debug.Log(fileData.Id);
```

#### [Delete File](https://beta.openai.com/docs/api-reference/files/delete)

Delete a file.

```csharp
var api = new OpenAIClient();
var result = await api.FilesEndpoint.DeleteFileAsync(fileData);
Assert.IsTrue(result);
```

#### [Retrieve File Info](https://beta.openai.com/docs/api-reference/files/retrieve)

Returns information about a specific file.

```csharp
var api = new OpenAIClient();
var fileData = await GetFileInfoAsync(fileId);
Debug.Log($"{fileData.Id} -> {fileData.Object}: {fileData.FileName} | {fileData.Size} bytes");
```

#### [Download File Content](https://beta.openai.com/docs/api-reference/files/retrieve-content)

Downloads the specified file.

```csharp
var api = new OpenAIClient();
var downloadedFilePath = await api.FilesEndpoint.DownloadFileAsync(fileId, "path/to/your/save/directory");
Debug.Log(downloadedFilePath);
Assert.IsTrue(File.Exists(downloadedFilePath));
```

### [Fine Tuning](https://beta.openai.com/docs/api-reference/fine-tunes)

Manage fine-tuning jobs to tailor a model to your specific training data.

Related guide: [Fine-tune models](https://beta.openai.com/docs/guides/fine-tuning)

The Files API is accessed via `OpenAIClient.FineTuningEndpoint`

#### [Create Fine Tune Job](https://beta.openai.com/docs/api-reference/fine-tunes/create)

Creates a job that fine-tunes a specified model from a given dataset.

Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.

```csharp
var api = new OpenAIClient();
var request = new CreateFineTuneRequest(fileData);
var fineTuneJob = await api.FineTuningEndpoint.CreateFineTuneJobAsync(request);
Debug.Log(fineTuneJob.Id);
```

#### [List Fine Tune Jobs](https://beta.openai.com/docs/api-reference/fine-tunes/list)

List your organization's fine-tuning jobs.

```csharp
var api = new OpenAIClient();
var fineTuneJobs = await api.FineTuningEndpoint.ListFineTuneJobsAsync();

foreach (var job in fineTuneJobs)
{
    Debug.Log($"{job.Id} -> {job.Status}");
}
```

#### [Retrieve Fine Tune Job Info](https://beta.openai.com/docs/api-reference/fine-tunes/retrieve)

Gets info about the fine-tune job.

```csharp
var api = new OpenAIClient();
var result = await api.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
Debug.Log($"{result.Id} -> {result.Status}");
```

#### [Cancel Fine Tune Job](https://beta.openai.com/docs/api-reference/fine-tunes/cancel)

Immediately cancel a fine-tune job.

```csharp
var api = new OpenAIClient();
var result = await api.FineTuningEndpoint.CancelFineTuneJobAsync(fineTuneJob);
Assert.IsTrue(result);
```

#### [List Fine Tune Events](https://beta.openai.com/docs/api-reference/fine-tunes/events)

Get fine-grained status updates for a fine-tune job.

```csharp
var api = new OpenAIClient();
var fineTuneEvents = await api.FineTuningEndpoint.ListFineTuneEventsAsync(fineTuneJob);
Debug.Log($"{fineTuneJob.Id} -> status: {fineTuneJob.Status} | event count: {fineTuneEvents.Count}");
```

#### [Stream Fine Tune Events](https://beta.openai.com/docs/api-reference/fine-tunes/events#fine-tunes/events-stream)

```csharp
var api = new OpenAIClient();
await api.FineTuningEndpoint.StreamFineTuneEventsAsync(fineTuneJob, fineTuneEvent =>
{
    Debug.Log($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
});
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
await foreach (var fineTuneEvent in api.FineTuningEndpoint.StreamFineTuneEventsEnumerableAsync(fineTuneJob))
{
    Debug.Log($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
}
```

### [Moderations](https://beta.openai.com/docs/api-reference/moderations)

Given a input text, outputs if the model classifies it as violating OpenAI's content policy.

Related guide: [Moderations](https://beta.openai.com/docs/guides/moderation)

The Moderations API can be accessed via `OpenAIClient.ModerationsEndpoint`

#### [Create Moderation](https://beta.openai.com/docs/api-reference/moderations/create)

Classifies if text violates OpenAI's Content Policy.

```csharp
var api = new OpenAIClient();
var response = await api.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
Assert.IsTrue(response);
```
