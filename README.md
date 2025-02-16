<p align="center">
  <a href="https://github.com/srdrsvml1986/NArchitectureTemplate/graphs/contributors"><img src="https://img.shields.io/github/contributors/kodlamaio-projects/nArchitecture.svg?style=for-the-badge"></a>
  <a href="https://github.com/srdrsvml1986/NArchitectureTemplate/network/members"><img src="https://img.shields.io/github/forks/kodlamaio-projects/nArchitecture.svg?style=for-the-badge"></a>
  <a href="https://github.com/srdrsvml1986/NArchitectureTemplate/stargazers"><img src="https://img.shields.io/github/stars/kodlamaio-projects/nArchitecture.svg?style=for-the-badge"></a>
  <a href="https://github.com/srdrsvml1986/NArchitectureTemplate/issues"><img src="https://img.shields.io/github/issues/kodlamaio-projects/nArchitecture.svg?style=for-the-badge"></a>
  <a href="https://github.com/srdrsvml1986/NArchitectureTemplate/blob/master/LICENSE"><img src="https://img.shields.io/github/license/kodlamaio-projects/nArchitecture.svg?style=for-the-badge"></a>
</p><br />

<p align="center">
  <a href="https://github.com/srdrsvml1986/NArchitectureTemplate"><img src="https://user-images.githubusercontent.com/53148314/194872467-827dc967-acee-4bca-88a2-59ed5695bebf.png" height="125"></a>
  <h3 align="center">nArchitecture Project
</h3>
  <p align="center">
    <!-- PROJECT_DESCRIPTION -->
    <!-- <br />
    <a href="https://github.com/srdrsvml1986/NArchitectureTemplate"><strong>Explore the docs »</strong></a>
    <br /> -->
    <!-- <br />
    <a href="https://github.com/srdrsvml1986/NArchitectureTemplate">View Demo</a>
    · -->
    <a href="https://github.com/srdrsvml1986/NArchitectureTemplate/issues">Report Bug</a>
    ·
    <a href="https://github.com/srdrsvml1986/NArchitectureTemplate/discussions">Request Feature</a>
  </p>
</p>

## 💻 About The Project

As Kodlama.io, we decided to share examples of completed projects. Inspired by Clean Architecture, nArchitecture is a monolith project that showcases advanced development techniques. The project includes Clean Architecture, CQRS, Advanced Repository, Dynamic Querying, JWT, OTP, Google & Microsoft Auth, Role-Based Management, Distributed Caching (Redis), Logging (Serilog), Elastic Search, [Code Generator](https://github.com/srdrsvml1986/NArchitectureTemplate.Gen) and much more. By contributing, you can support the project and learn new things.

### Built With

[![](https://img.shields.io/badge/.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/tr-tr/dotnet/welcome)

## ⚙️ Getting Started

To get a local copy up and running follow these simple steps.

### Prerequisites

- .NET 7

### Installation

1. Clone the repo
   ```sh
   git clone --recurse-submodules https://github.com/srdrsvml1986/NArchitectureTemplate.git
   ```
2. Configure `appsettings.json` in WebAPI.
3. Run `Update-Database` command with Package Manager Console in WebAPI to create tables in sql server.

- Run the following command to update submodules
  ```sh
   git submodule update --remote
   ```

## 🚀 Usage

1. Run example WebAPI project `dotnet run --project src\rentACar\WebAPI`

### Analysis

1. If not, Install dotnet tool `dotnet tool restore`.
2. Run anaylsis command `dotnet roslynator analyze`

### Format

1. If not, Install dotnet tool `dotnet tool restore`.
2. Run format command `dotnet csharpier .`

## 🚧 Roadmap

See the [open issues](https://github.com/srdrsvml1986/NArchitectureTemplate/issues) for a list of proposed features (and known issues).

## 🤝 Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the project and clone your local machine
2. Create your Feature Branch (`git checkout -b <Feature>/<AmazingFeature>'`)
3. Develop
4. Commit your Changes (`git add . && git commit -m '<SemanticCommitType>(<Scope>): <AmazingFeature>'`)
   💡 Check [Semantic Commit Messages](./docs/Semantic%20Commit%20Messages.md)
5. Push to the Branch (`git push origin <Feature>/<AmazingFeature>`)
6. Open a Pull Request

Contributing on Core Packages With This Repo:

1. Fork the [nArchitecture.Core](https://github.com/srdrsvml1986/NArchitectureTemplate.Core) project
2. Locate to `src/corePackages` path (`cd .\src\corePackages\`)
3. Add your forked nArchitecture.Core repository remote address (`git remote add <YourUserName> https://github.com/<YourUserName>/nArchitecture.Core.git`)
4. Create your Feature Branch (`git checkout -b <Feature>/<AmazingFeature>'`)
5. Develop
6. Commit your changes (`git add . && git commit -m '<SemanticCommitType>(<Scope>): <AmazingFeature>'`)
   💡 Check [Semantic Commit Messages](./docs/Semantic%20Commit%20Messages.md)
7. Push to the branch (`git push <YourUserName> --set-upstream HEAD:refs/heads/<Feature>/<AmazingFeature>`)
8. Open a Pull Request

If your pull request is accepted and merged:

9. Locate to `src/corePackages` path (`cd .\src\corePackages\`)
10. Switch to main branch `git checkout main`
11. Locate root path `/` path (`cd ..\..\`)
12. Pull repo and submodule `git submodule update --remote`
13. Commit your changes (`git add . && git commit -m 'build(corePackages): update submodule'`)
14. Push to the Branch (`git push origin <Feature>/<AmazingFeature>`)
15. Open a Pull Request

## ⚖️ License

Distributed under the MIT License. See `LICENSE` for more information.

## 📧 Contact

**Project Link:** [https://github.com/srdrsvml1986/NArchitectureTemplate](https://github.com/srdrsvml1986/NArchitectureTemplate)

<!-- ## 🙏 Acknowledgements
- []() -->