# S2C: Space To Clean
AI agents compete to clean a 2D grid where some cells are dirty and some are walls (where agents cannot go.)
This program is based on the vacuum cleaner agent problem in Chapter 1 of the AI book by Norvig.

![alt text](https://i.imgur.com/IgIPrcD.png "S2C Screenshot")

**Table of Contents**

* [Concepts](#Concepts)
  * [The Space](#the-space)
  * [The Agent](#the-agent)
  * [Objective](#objective)
* [Implementation](#Implementation)
  * [Agent Contract](#Agent-Contract)
* [Installation](#Installation)
  * [From Code](#from-code)
  * [From Binaries](#from-binaries)

# Concepts

## The Space
A two-dimensional grid containing three kinds of rectangular cells - Clean, Dirty, and Wall.
In the beginning the problem/game starts with random placements of a few clean, dirty and wall cells.

One or more agents are also placed on to the grid. The game proceeds in rounds where every agent gets one move per round.

## The Agent
Every agent is a separate program which implements a contract, allowing the simulation/game controller to communicate with it. Each agent has an ID, and a location (row, column) on the grid. The agent also contains logic/programming to act when it receives a request for a move from the controller.

The controller provide some information (location, and whether the location is dirty) to the agent and asks it for a move. Every agent gets one move per round.
The agent has two allowed moves:
* **MOVETO**: move to another cell adjacent to the current cell
* **CLEAN**: clean the current cell if it is dirty.

## Objective
The objective is to clear as many dirty cells as possible is as few rounds as possible. The game ends when all dirty cells are cleared.

# Implementation
The current implementation of the space, simulation and most agents is as a C# .NET Core program. The UI is built using HTML and a few JS libraries. The program should work on windows, macos and linux-64. The program also supports a cleaning agent written as a REST web service, which gets called via a HTTP Proxy Agent. This allows implementations of agents in any programming language where one can implement a web service.

## Agent Contract
An agent implements the following methods/endpoints.
* **Set Agent Id**: Accept the numeric agent id assigned to it, and creat a new agent.
* **Set Space Size**: Accept the size (row X column) of the space to clean.
* **Next Command**: Provide the next command to be run in the currrent round.
* **Command Result**: Accept the result of the command, when applied to the space.

The simplest example of an agent implementation is https://github.com/abhishekmishra/s2c/blob/master/S2CServer/S2CCore/SimpleCleaningAgent.cs

# Installation

## From code
Install dotnet core runtime for your platform - see https://dotnet.microsoft.com/download
```bash
$ git clone https://github.com/abhishekmishra/s2c.git
$ cd s2c/S2CServer
$ dotnet run -p ./S2CServices/S2CServices.csproj
```

Now open browser at https://localhost:5003/

## From binaries
TBD
