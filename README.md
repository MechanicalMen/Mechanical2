Mechanical v2
=============

Project Info
------------

### Why another library?
This library was developed to ease many everyday tasks, like writing preconditions, saving in a structured manner, and countless more.

### Building
You will need to build using at least a C#5/.NET4.5 compiler (even if your target is below .NET 4.5). The libraries are currently maintained using Visual Studio 2012 Express for Web (Desktop works just as fine, if you don't mind loosing the Silverlight library).

### License
MIT (unless otherwise stated)


Namespaces
----------

### Core
Utilities used by all other namespaces. Helps working with IDisposable, enums, string and substrings, ... etc.

### Conditions
A framework for preconditions (e.g. testing your parameters) you'll love to use! It's highlights are:
* Fluent syntax
* Customizable: specify your own exceptions if you don't like the default ones (or their message)
* Extendable: write your own conditions, even from external assemblies
* File and line information for all exceptions (finally! :)
* Easily append extra information to any kind of exception (like current state, intermediate results, ... etc.)

### Collections
Base classes for implementing your own collections. Make your code more expressive (think: Car.Wheels.Add...), or make generated values more accessible (than IEnumerable).
