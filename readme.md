# DotnetSnes Read Me

## Quick build the example project

Must be built in Linux (WSL works too). Tested on Ubuntu 24.04.1 LTS.

1. Clone the DotnetSnes repository
1. Do a recursive submodule initialization on the clone to get dependent projects
    * `git submodule update --init --recursive`
    * The dependent projects may require authentication via SSH (https://docs.github.com/en/authentication/connecting-to-github-with-ssh)
1. `cd` into the `pvsneslib/` directory and run `make`
1. `cd` into the `src/DotnetSnes.Example.HelloWorld/` directory and run `make`


### Dependencies needed to install on WSL Ubuntu for make

* `export PVSNESLIB_HOME=(yourpath)/DotnetSnes/pvsneslib`
* `sudo apt-get update` (if needed)
* `sudo apt-get install cmake`
* `sudo apt-get install g++`
* `sudo apt-get install dotnet-sdk-8.0`