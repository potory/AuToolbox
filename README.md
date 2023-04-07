# Table of contents

- [AuToolbox](#autoolbox)
    - [Installation](#installation)
    - [Usage](#usage)
    - [Commands](#commands)
    - [Configuration](#configuration)
- [Macroses](#macroses)
    - [Basic Macroses](#basic-macroses)
    - [Config macroses](#config-macroses)
- [Prompthing](#prompthing)
    - [Prompthing File Format](#prompthing-file-format)
    - [Example](#example)
    - [Prompthing macroses](#prompthing-macroses)

# AuToolbox
AuToolbox is a command-line application that automates the process of image generation with the AUTOMATIC1111 Stable Diffusion API. It allows users to define a series of generation steps by configuring requests in a JSON file, with support for SonScript macros language. The generated images are saved in a folder corresponding to the index of the generation step, and can be used as input for subsequent steps.

AuToolbox is a C# .NET 7 application that uses the Newtonsoft Json library for handling JSON files. It also features the ConsoleFramework and Prompthing libraries for command-line application and text generation, respectively. Additionally, it utilizes a macros language called SonScript that enables the inclusion of resolvable macroses in text, which is used for generating requests to the API.

AuToolbox supports both Text2Image and Image2Image generation steps, and images can be passed between steps to create more advanced image processing workflows.

## Installation
To install AuToolbox, you can either download a pre-compiled binary from the releases page on GitHub or build it from source code by following these steps:

1. Clone the repository to your local machine using Git:

```bash
git clone https://github.com/potory/AuToolbox.git
```
2. Open the project in your preferred IDE, such as Visual Studio or JetBrains Rider.
3. Build the project using .NET SDK 7 or later.
4. Once the build is complete, you will find the AuToolbox.exe binary in the bin folder.

## Usage
To use AuToolbox, you can run it from the command prompt/terminal.

### Running from Command Prompt/Terminal
Navigate to the directory where AuToolbox was extracted and open a command prompt or terminal. Then, type the following command:

```
AuToolbox.Console [command] [options]
```
Where [command] is one of the available commands and [options] are the options specific to that command. You can see a list of available commands by typing help.

To get help on a specific command, type:

```
AuToolbox.Console help [command]
```
## Commands

`help`

Displays the help information for AuToolbox or a specific command.

`images-generate`

Generates a series of images using the specified configuration file. The command requires two parameters: the path to the configuration file and the number of images to generate. You can specify additional parameters, such as the output folder, using the [options] parameter.

For example:
```
AuToolbox.Console images-generate config.json 10
```

This command will generate 10 images using the config.json file.


`Other commands`

AuToolbox has other commands that can be used for different purposes. You can see a full list of available commands by typing `help`.

## Configuration
AuToolbox uses JSON files to specify the generation steps and their associated parameters. Here is an example configuration file:

```json
[
  {
    "prompt": "a cat",
    "negative_prompt": "bad quality",
    "width": 512,
    "height": 512
  },
  {
    "init_images": [
      "#previousResult()"
    ],
    "width": 1024,
    "height": 1024
  }
]
```
This configuration file specifies two generation steps. The first step generates an image with the prompt "a cat" and a negative prompt of "bad quality" with a width and height of 512 pixels. The second step generates an image with the previous image as its initial image (specified by the "#previousResult()" macro), a width of 1024 pixels, and a height of 1024 pixels.

AuToolbox also includes a `Defaults/DefaultRequest.json` file that serves as a default set of parameters for any request. Configuration files can override any or all of these parameters, **and each subsequent generation step will inherit the parameters of the previous step unless they are overridden.**

# Macroses
## Basic Macroses
### #mult and #multiply
The #mult and #multiply macroses are used to multiply any number of parameters together.
```
Example: #mult(2, 4, 6) will output 48, which is 2 x 4 x 6.
```

### #eight
The #eight macro is used to round the input number down to the closest multiple of 8.
```
Example: #eight(17) will output 16, which is the closest multiple of 8 to 17.
```

### #int
The #int macro is used to convert a floating-point number to an integer.
```
Example: #int(3.14159) will output 3, which is the integer value of 3.14159.
```

### #replace
The #replace macro is used to replace specific strings in the input. It takes three arguments: the source string, the old value, and the new value. For example, #replace('Hello, world!', 'world', 'John') would return "Hello, John!".
```
Example: #replace('Hello, world!', 'world', 'Jack') will output 'Hello, Jack!', which is the original string with 'world' replaced by 'Jack'.
```

### #linefrom
The #linefrom macro is used to select a random line from a file. It takes one argument, which is the path to the file (relative to the AuToolbox executable or absolute). For example, #linefrom('file.txt') would return a randomly selected line from the "file.txt" file.
```
Example: #linefrom('data.txt') will read a random line from the 'data.txt' file and output it.
```

### #oneof
The #oneof macro is used to choose a random value from a list of arguments.
```
Example: #oneof('cat', 'dog', 'bird') will randomly choose one of the three options and output it.
```

### #append
The #append macro is used to concatenate strings together. It takes any number of arguments and joins them together.
```
Example: #append('Hello', ' ', 'world', '!') will output 'Hello world!', which is the concatenation of the four string arguments.
```

## Config macroses

### #previousResult
The #previousResult macro is used to get the image result from the previous generation step.
```
Example: If the previous generation step produced an image, #previousResult() can be used to retrieve that image and use it in the current generation step.
```

### #prompthing
The #prompthing macro is used to retrieve a text value from the Prompthing file. It takes one argument, which is the path to the Prompthing file.
```
Example: #prompthing('prompthing.json') will compile 'prompthing.json' file and randomly choose one of the templates that are not marked as "isSnippet":true.
```

### #source
The #source macro is used to retrieve a value from the previous step or defaults (if any). If there is no value stored in the previous step or defaults, it will return nothing.
```
Example: If a default configuration file has a "prompt" parameter set to "a dog", #source() can be used to retrieve that value and use it in the current generation step.
```

# Prompthing

To use the Prompthing feature, you'll need to create a Prompthing file, which is a JSON configuration file that defines templates and categories.

## Prompthing File Format
The Prompthing file consists of two main sections: Templates and Categories.

### Templates
Templates are the main building blocks of your generated text. Each template defines a unique piece of text that can reference other templates or categories using the #t() macro.

Here's an example of a Templates section:
```json
"templates": [
  {
    "name": "sourceTemplate",
    "isSnippet": false,
    "template": "a {{gender}} standing in #t('location') holding {{item}}"
  },
  {
    "name": "location",
    "isSnippet": true,
    "template": "{{size}} {{place}}"
  }
]
```
Each template has the following properties:

- name: a unique name for the template.
- isSnippet (optional): a boolean indicating whether the template should be treated as a snippet (a small piece of text that can be inserted into other templates and not will exist in output as a separate template).
- template: the text of the template, which can include references to other templates or categories using the #t() macro.

In the example above, the sourceTemplate template references the location template using the #t() macro.

Categories
Categories are collections of values that can be used in templates. Each category has a name and an array of values.

Here's an example of a Categories section:

### Categories

```json
"categories": [
  {
    "name": "size",
    "values": [
      "a big",
      "a small"
    ]
  },
  {
    "name": "place",
    "values": [
      "park",
      "house"
    ]
  },
  {
    "name": "gender",
    "values": [
      "man",
      "woman"
    ]
  },
  {
    "name": "item",
    "values": [
      "a book",
      "a smartphone"
    ]
  }
]
```

Each category has the following properties:

- name: a unique name for the category.
- values: an array of values that can be referenced in templates.

#### Alternative:

- name: a unique name for the category.
- path: an alternative for 'value', must be path to a .json file, that contains an array of values.

In the example above, there are categories for size, place, gender, and item, each with a set of values that can be used in templates.

```json
{
    "name": "item",
    "path": "D:\\items.json"
}
```

### Weighted Values
In addition to plain strings, values can also be weighted to increase or decrease their chances of being randomly chosen. To specify a weighted value, use the following format:

```json
{
    "name": "item",
    "values": [
        {
            "weight": 1,
            "text": "a book"
        },
        {
            "weight": 2,
            "text": "a phone"
        }
    ]
}
```

In this example, the value "a phone" is twice as likely to be randomly chosen as "a book".

## Example
Here's an example of a complete Prompthing file that uses the templates and categories from the previous examples:

```json
{
  "templates": [
    {
      "name": "sourceTemplate",
      "isSnippet": false,
      "template": "a {{gender}} standing in #t('location') holding {{item}}"
    },
    {
      "name": "location",
      "isSnippet": true,
      "template": "{{size}} {{place}}"
    }
  ],
  "categories": [
    {
      "name": "size",
      "values": [
        "a big",
        "a small"
      ]
    },
    {
      "name": "place",
      "values": [
        "park",
        "house"
      ]
    },
    {
      "name": "gender",
      "values": [
        "man",
        "woman"
      ]
    },
    {
      "name": "item",
      "values": [
        "a book",
        "a smartphone"
      ]
    }
  ]
}
```

## Prompthing macroses
### #template, #temp, and #t
The #template, #temp, and #t macroses are used to insert another template into the current one. They take one argument, which is the name of the template to insert. However, be careful when using these macroses, as they do not have a recursive check.
```
Example: #template('my_template') can be used to insert the 'my_template' template into the current template being processed.
```