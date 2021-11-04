<div id="top"></div>

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]



<!-- PROJECT LOGO -->
<div align="center">
  <a href="https://github.com/LilianSchall/Synchro">
    <img src="images/Synchro.png" alt="Logo" width="960" height="540">
  </a>

  <h3 align="center">SYNCHRO</h3>
  This is the official repository for Synchro Discord Bot. Let the public of a voice channel be the dj of it, by playing music through Synchro. 
  <p align="center">
    
    <a href="https://https://github.com/LilianSchall/Synchro"><strong>Explore the docs »</strong></a>
    <a href="https://github.com/LilianSchall/Synchro/">View Demo</a>
    ·
    <a href="https://github.com/LilianSchall/Synchro/issues">Report Bug</a>
    ·
    <a href="https://github.com/LilianSchall/Synchro/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

Synchro is an open-source project that started as a community bot designed to replace Rythm, following its ban request from Youtube. Synchro is aiming to be deployed by the people for the people ! 
Any pull request or issue is welcomed !

<p align="right">(<a href="#top">back to top</a>)</p>



### Built With

This project has been built with the following prerequisites

* [C# .NET Core 3.1](https://dotnet.microsoft.com/download)
* [Discord.net](https://docs.stillu.cc/guides/introduction/intro.html)
* [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode)
* [python3](https://www.python.org/downloads/)
* [yt-dlp](https://github.com/yt-dlp/yt-dlp)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

Please install the following required tools and frameworks:
* Ubuntu
  ```sh
    sudo apt-get install python3 python3-pip libsodium-dev opus-tools &&
    python3 -m pip install -U yt-dlp &&
    ...
    
  ```

### Installation

_Below is an example of how you can instruct your audience on installing and setting up your app. This template doesn't rely on any external dependencies or services._

1. Creaye an application on Discord developer portal ! [https://discord.com/developers/applications](https://discord.com/developers/applications)
2. Clone the repo
   ```sh
   git clone https://github.com/LilianSchall/Synchro.git
   ```
3. In the repo, compile the project.
   ```sh
   dotnet publish -c Release
   ```
4. Add the token to your environnment as following:
   ```sh
   export DISCORDTOKEN="your_token"
   ```
5. ...
<p align="right">(<a href="#top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

When your Synchro is finally ready, type ;help in your channel to see the current command available !



<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap

- [] Work on docker containerization
- [] Add whitelist channel command
- [] Work on CI
- [] Add remove from queue command
- [] Work on queue bug 

See the [open issues](https://github.com/LilianSchall/Synchro/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/SynchroFeature`)
3. Commit your Changes (`git commit -m 'Adding features to Synchro !'`)
4. Push to the Branch (`git push origin feature/SynchroFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Lilian Schall - [@your_twitter](https://twitter.com/lilixns)

Project Link: [https://github.com/LilianSchall/Synchro](https://github.com/LilianSchall/Synchro)

<p align="right">(<a href="#top">back to top</a>)</p>







<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/LilianSchall/Synchro.svg?style=for-the-badge
[contributors-url]: https://github.com/LilianSchall/Synchro/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/LilianSchall/Synchro.svg?style=for-the-badge
[forks-url]: https://github.com/LilianSchall/Synchro/network/members
[stars-shield]: https://img.shields.io/github/stars/LilianSchall/Synchro.svg?style=for-the-badge
[stars-url]: https://github.com/LilianSchall/Synchro/stargazers
[issues-shield]: https://img.shields.io/github/issues/LilianSchall/Synchro.svg?style=for-the-badge
[issues-url]: https://github.com/LilianSchall/Synchro/issues
[license-shield]: https://img.shields.io/github/license/LilianSchall/Synchro.svg?style=for-the-badge
[license-url]: https://github.com/LilianSchall/Synchro/blob/main/LICENSE.txt
