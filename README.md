# TaskRunner

Designed to help run scheduled tasks under Windows.

TaskRunner launches an program with and captures any output. If there is any output or the program exits with a non-zero exit code, then an e-mail is generated.

## Example use

```
TaskRunner -t "Admin <administrator@example.com>" -- MyBackupScript.pl
```

## Switches

- ```-s``` *mode* or ```--send``` *mode*  
Specify when to send an e-mail. *Mode* can be one of three values:
  - ```Always```- send an e-mail whether no matter what the result of the task.
  - ```OnFailure``` - send an e-mail only if the task exits with a non-zero exit code.
  - ```OnFailureOrOutput``` - send an e-mail if the task exits with a non-zero exit code, or if it prints any output to either stderr or stdout.

- ```-e``` *exitcodes* or ```--exitcode``` *exitcodes*  
Specify what exit codes should be considered as successful. *Exitcodes* is a comma separated list of either single values or ranges separated with a hyphen. It cannot contain any spaces.

- ```-h``` *host* or ```--host``` *host*  
Specify the SMTP host. Currently only unauthenticated connections on port 25 are supported.

- ```-f``` *address* or ```--from``` *address*  
Specify the sender address to send the e-mail from.

- ```-t``` *address* or ```--to``` *address*  
Specify the recipient address to send the e-mail to.

- ```-c``` or ```--configure```  
Store any e-mail configuration values in the registry for future invocations.

- ```--```  
Separates any switches for the application from the program to run and its arguments.

E-mail addresses can be just a bare e-mail address or can include a name and an e-mail address in angle brackets, e.g. ```-f "Joe Bloggs <joe.bloggs@example.com>"```

## Installation

It is recommended to run the program initially with the ```--configure``` switch and at least an e-mail host and from address to avoid having to specify them every time. Once configured, these options no longer need to be specified on the command-line. If they are specified on the command line, then they will override any values in the registry.

```
TaskRunner.exe --configure --host mail.example.com --from "Alerts <alerts@example.com>" --to "Admin <administrator@example.com>"
```
