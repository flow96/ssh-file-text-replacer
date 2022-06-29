# SSH file text replacer

## Installation
- Download one of the <a href="https://github.com/flow96/ssh-file-text-replacer/releases">release versions</a> (e.g. v1.0-linux-x64.zip)
- Configure the `appsettings.json` to your needs (included in the release zip file)
- Run the SshFileTextReplacer executable

## Configuration
The `appsettings.json` file needs to be in the same folder as the executable.<br />
The appsettings.json holds all relevant information that is needed to do the replacement.

### Line to find
That is the line the program will search for and replace it
```
  "lineToFind": "Dummy text 1.1.1.1",
```

### Line to replace
That is the line that will replace the found line (see above)
```
  "lineToReplace": "# Dummy text 1.1.1.1",
```

### File path
The path to the file where to replace the given lines<br/>
Can be absolute or relative.
```
  "filePath": "./test.txt",
```

### Commands to run afterwards
If you want to execute commands after changing the file (for example restarting a service) you can add the commands to this array.
```
  "commandsToRunAfterwards": [
    "service restart apache2",
    "ls",
    "..."
  ]
```

### Server settings
These are the ssh settings that are needed to connect to the server via ssh.
```
  "serverSettings": {
    "host": "192.168.178.1",
    "username": "",
    "password": ""
  }
```
