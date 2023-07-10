    # WalletWeb

    This repository contains a web UI for interacting with the WalletApi, which is an n-tier application located at [WalletApi](https://github.com/coommark/WalletApi). The WalletWeb project consumes the WalletApi using .NET's `System.Net.Http.HttpClient` to perform authentication and transactions.

    ## Prerequisites

    Before running the WalletWeb project, make sure you have the following installed:

    - Visual Studio 2019
    - SQL Server 2019

    ## Getting Started

    To run the WalletWeb project locally, follow these steps:

    1. Clone this repository to your local machine:

    ```bash
    git clone https://github.com/coommark/WalletWeb.git
    ```

    2. Open the solution file `WalletWeb.sln` in Visual Studio 2019.

    3. Set up the connection string in the `appsettings.json` file. Modify the `DefaultConnection` section with your SQL Server credentials:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=<server>;Database=<database>;User Id=<user>;Password=<password>;"
    }
    ```

    Replace `<server>`, `<database>`, `<user>`, and `<password>` with your SQL Server details.

    4. Build the solution to restore dependencies and compile the project.

    5. Run the project using Visual Studio's debugging feature.

    6. The WalletWeb application should now be running locally. Open your web browser and navigate to the following URL:

    ```
    http://localhost:<port>
    ```

    Remember to replace `<port>` with the appropriate port number where the application is running.

    Please note that this repository is designed to work in conjunction with the WalletApi. Ensure that the WalletApi is running and accessible before using the WalletWeb application.

    For more information on how to set up and use the WalletApi, refer to its [documentation](https://github.com/coommark/WalletApi).
