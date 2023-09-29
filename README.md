## H3llocker
Windows ransomware locker AES implemented in C#, integrated with Discord webhook.

## Disclaimer
This locker was created for educational purposes and for security and intrusion tests. Do not use this locker for illegal activities. I am not responsible for any consequences that occur from using my code.

<p align="center">
  <img src="https://raw.githubusercontent.com/kasp4rov/H3llocker/main/images/desktop.png" alt="Desktop" width="800"/>
</p>

## Summary
Once executed, the Locker code will encrypt personal files using AES encryption. The decryption key will be sent to the attacker via a Discord Webhook. The victim will also be notified about the successful execution of the file.

<p align="center">
  <img src="https://raw.githubusercontent.com/kasp4rov/H3llocker/main/images/webhook.png" alt="Discord" />
</p>

## Compiling

To compile, run the following command: 
<pre>C:\H3llocker> dotnet restore </pre>

Import the code using Visual Studio and compile.

![Visual Studio](https://raw.githubusercontent.com/kasp4rov/H3llocker/main/images/image.png)
