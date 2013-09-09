Mechanical v2
=============

Project Info
------------

### Why another library?
This library was developed to ease many everyday tasks, like writing preconditions, saving in a structured manner, and countless others.

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

### MagicBag
A unique take on IoC containers. It has basic functionality, but is very extendable.
* Very simple, and strongly typed interface (2 methods only: Pull<T> and IsRegistered<T>)
* Designed to be easily integrated with other IoC containers
* Immutable container: if you registered it, it's there. Based on how and what you register, this may provide thread-safety
* Container hierarchy: write less code! Blacklist, whitelist, or simply replace mappings
* Constructor, Property, Field and Method injection is possible
* Mixing custom and injected parameters is also possible
* Types like Func<T>, Lazy<T>, IEnumerable<T> and others, as well as your own, can be automatically generated

### DataStores
A unique take on serialization: 
* Strongly typed
* High level of abstraction, but can manipulate low level data
* Takes the best parts of common .NET classes: The ease of use from XElement, BinaryReader/Writer, and speed from the latter, as well as XmlReader/Writer.
* Hides the raw data format (text or binary), the file format (xml, json, ...), and optionally (through a MagicBag) the data representation (think ISO8601 for DateTime, cultures for numbers, ... etc.)
* Through the use of a MagicBag, changing the DateTime format is as easy as specifying a new Mapping. This can be done without touching a single line of serialization code anywhere

### Collections
Base classes for implementing your own collections. Make your code more expressive (think: Car.Wheels.Add...), or make generated values more accessible (than IEnumerable).

### IO
Replaces the functionality of StreamReader/Writer and BinaryReader/Writer.
* Interfaces abstract the handling of raw binary or textual data.
* Lightweight and reusable implementations
