# Wetonomy
Dao Framework build on top of [Apocryph](https://github.com/comrade-coop/apocryph).

Wetonomy gives you primitives to set up your organization workflow in the blockchain. It is build using**C#** and the [Actor Model](https://en.wikipedia.org/wiki/Actor_model)

For now, it runs with Apocryph **TestBed** until the Apocryph network is finalized.

## Getting Started

### Prerequisite
- Install [Azure Functions Core Tools v3](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#v2)
- Install [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- Install [Docker](https://docs.docker.com/install/)

### Get git submodules
Execute `git clone --recurse-submodules -j8 https://github.com/comrade-coop/wetonomy.git` to clone the repo along with submodules

Once you've done that, you can run the **Wetonomy.FunctionApp** which bootstraps the organization.

## Wetonomy Overview
We can divide the organization architecture in 3 modules
1. Governance
2. Token Flow
3. Work Management

### Governance
Governance module contains **Members**, **Groups** and **Voting**
* **Member** is an agent that represent users in the organization. Being a member you get the permissions to vote, track work, create new decisions in the voting etc.

* **Group** is an agent that gathers together Members so that they have common permissions. This is implemented via **message forwarding**

* **Voting** is the central governance mechanism. It can perform any kind of action as long as this is defined in the organization genesis, if you don't give the voting permissions it cannot operate.

### Token Flow
These are the dynamics of the organizations. For example when you track work a reward token is minted, when investor makes investment he receives debt token, when salary is paid tokens are transferred etc.
The main idea is everything to be automated and once created the organization works from itself.
The core components here are **TokenManager** and **TokenActionAgent**
* **TokenManager** is responsible for minting, transferring, burning and keeping track of tokens.
* **TokenActionAgent** is responsible for custom defined actions. You can assign to it different **Triggers** for example when member tracks work TokenActionAgent can mint tokens for reward.
![alt text](https://github.com/comrade-coop/wetonomy/blob/master/docs/TokenFlow.png "Token Flow Diagram")

### Work Management
This module is responsible for work tracking. It could be tracking hours, predefined fix hours(monthly salary) or bounties for example. The main idea is the reward for work to be tokens and that is how Work Management is connected to system dynamics.
