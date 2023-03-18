
# AutomaticToolbox

AutomaticToolbox is an open-source C# command-line interface (CLI) application designed to automate the process of generating similar image data using the AUTOMATIC1111 Stable Diffusion API. The application utilizes the Prompthing library for generating customized prompts and includes the SonScript macro engine for automating tasks.

AutomaticToolbox is primarily designed for generating similar data, which can be used for purposes such as model training or generating regularization images. The CLI interface makes AutomaticToolbox easy to integrate into larger workflows and scripts.

- [AutomaticToolbox](#automatictoolbox)
    * [Getting started](#getting-started)
        + [Preparing configs](#preparing-configs)
        + [Running an application](#running-an-application)
    * [Available macroses for a config and prompt files](#available-macroses-for-a-config-and-prompt-files)
        + [#mult](#mult)
        + [#append](#append)
        + [#replace](#replace)
        + [#eight](#eight)
        + [#oneof](#oneof)
        + [#linefrom](#linefrom)
        + [#int](#int)
        + [#prompthing](#prompthing)
    * [Config-exclusive macroses](#config-exclusive-macroses)
        + [#source](#source)
    * [Prompt-exclusive macroses](#prompt-exclusive-macroses)
        + [#template](#template)

## Getting started

### Preparing configs

To generate an image, create a `.json` config file. Here is an example of a simple config file for generating a cat in two iterations:
```json
{
  "iteration1": {
    "prompt": "a cat",
    "width": 512,
    "height": 640
  },
  "iteration2": {
    "width": 1024,
    "height": 1280
  }
}
```
In the first iteration, the prompt property is set to `a cat` and `width` and `height` to `512` and `640`, respectively. In the second iteration, the `width` and `height` are changed to `1024` and `1280`, respectively.

> **Warning**<br>
**AutomaticToolbox** always generates the first iteration image with the `txt2img` algorithm and uses this image as a base for further iterations and `img2img` processing.

### Running an application
1. Add an `--api` argument to your AUTOMATIC1111 `webui-user.bat` file in a `set COMMANDLINE_ARGS` section
2. Download the `AutomaticToolbox` application and extract it to a directory on your computer.
3. Open a text editor and create a new file named `run.bat` in the same directory as the main `AutomaticToolbox.Console.exe` file.
4. Copy the following code into `run.bat`:
```batch
set CONFIG=Configs/GenerationConfigs/example.json
set COUNT=5

@Echo off
:: GetDate5.cmd
:: Display date and time independent of OS Locale, Language or date format.
For /f "delims=" %%A in ('powershell get-date -format "{yyyy-MM-dd_HH-mm}"') do @set _dtm=%%A
Echo The current date/time is: %_dtm%

AutomaticToolbox.Console.exe images-generate %CONFIG% %COUNT% --output=Output/Images/%_dtm%
```
5. Edit the `CONFIG` variable to point to your desired configuration file. The example above assumes that you have a `GenerationConfigs` directory in the same directory as the `run.bat` file, and that you have an `example.json` file inside that directory.
6. Edit the `COUNT` variable to set the number of images you want to generate.
7. Save the `run.bat` file.
8. Run `AUTOMATIC111` and wait for it to initialize.
9. Run the `run.bat` file and wait for the image generation process to finish. The generated images will be saved in the `Output/Images` directory, in a subdirectory named with the current date and time.

## Available macroses for a config and prompt files

### #mult

`#mult()` can be used to multiply values. For example:
```json
"width": "#mult(#source(), 2)",
"height": "#mult(#source(), 2)"
```
This will make a picture twice as big as it was before.

### #append

`#append()` macro can be used to append text to another. For example:

```json
"prompt": "#append(#source, ' in a big house')"
```

This will add the text ` in a big house` to the end of the source prompt.

### #replace

`#replace()`  macro can be used to replace text with another. For example:

```json
"prompt": "#replace(#source, 'man', 'woman')"
```

This will replace the word `man` with `woman` in the source prompt.

### #eight

`#eight()` macro can be used to round down a number to the nearest divisor of eight. For example:

```json
"width": "#eight(514)"
```

This will set the `width` value to `512`.

### #oneof

`#oneof()` macro can be used to randomly choose one of the given values. For example:

```json
"prompt": "#oneof('a cat', 'a dog', 'a crow')"
```

This will randomly set the prompt value to either `a cat`, `a dog`, or `a crow`.

### #linefrom

`#linefrom()` macro can be used to randomly choose one of the lines from a specified file. For example:

```json
"prompt": "#linefrom('example.txt')"
```

This will randomly set the `prompt` value to one of the lines from an `example.txt` file, which is located in the same directory as the `AutomaticToolbox.Console.exe` file.

### #int

`#int()` macro can be used to round down a fractional number to an integer. For example:

```json
"width": "#int(512.55)"
```

This will set the `width` value to `512`.

### #prompthing

`#prompthing()` macro generates a text from a Prompthing template file. For example:

```json
"prompt": "#prompthing('Configs/PrompthingConfigs/example.json')"
```

This compiles templates from the example.json file located in `Configs/PrompthingConfigs/` and randomly selects one of them to generate a prompt.

## Config-exclusive macroses

### #source
`#source()` can be used to get the value from the same field of the config on a previous iteration. For example:
```json
"width": "#source()"
```
This will set the same value for the width as it was before.<br>
> **Warning**<br>
This macro does not work for a **Prompthing** templates

> **Warning**<br>
Additionally, note that it is not necessary to set value like this manually as every subsequent iteration will use the config file from the previous iteration with all its values.

## Prompt-exclusive macroses

### #template

`#template()` or `#t()` macro can be used to retrieve another template from the same config and insert it into a string. For example:

```json
  "templates": [
    {
      "name": "sourceTemplate",
      "template": "a {{gender}} standing in #t('location') holding {{item}}"
    },
    {
      "name": "location",
      "template": "{{size}} {{place}}"
    }
  ]
```

Here, the template with the name `sourceTemplate` will be compiled to a template that looks like this:

```
a {{gender}} standing in {{size}} {{place}} holding {{item}}
```

The `#t('location')` macro call will retrieve the `location` template and insert its contents into the string. The resulting prompt could be, for example:

```
a woman standing in a big house holding a book
```
