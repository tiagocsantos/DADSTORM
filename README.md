# Project - Design and Implementation of Distributed Applications

The goal of this project is to design and implement DADSTORM, a simplified implementation of a fault-tolerant real-time distributed stream processing system. A stream processing application is composed of a set of operators, each transforming a stream of tuples that is submitted to the operator’s input and that is output by the operator in its processed form. The stream engine will support a limited set of operators, but will have to provide strong guarantees on the correctness of the distributed computation and on performance predictability in presence of failures. 
Tuples are records composed of an arbitrary number of ordered fields, each one of them being, for simplicity, a string. 
The project should be implemented using C# and .Net Remoting using Microsoft Visual Studio and the CLR runtime 

# Description:
https://fenix.tecnico.ulisboa.pt/downloadFile/1689468335570372/DAD-project-description-published-v2.pdf

# To Run:

- Open in Visual Studio
- Build the Solution
- Press Start 
   -  it will lauch a Process Creation Service on your machine and then lauch a PuppetMaster from where it is possible to load Configuration Files
   
# Implementation 

- All requested features were implemented
