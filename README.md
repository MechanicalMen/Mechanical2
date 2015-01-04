Mechanical v2
=============

General Info
------------

### Why another library?
To ease many everyday tasks, like checking preconditions, saving data in a structured manner, writing cross-platform code, and countless others.

### Building
You will need to build using at least a C#5/.NET4.5 compiler (even if your target is below .NET 4.5). The libraries are currently maintained using Visual Studio 2013 Community.

### License
MIT (unless otherwise stated)

Projects
--------
I do not currently use portable libraries, therefore each platform has it's own project. The two main groups of projects are:
* Mechanical2: this is the main library. The idea is that the main library should provide basic, mostly self-contained tools. It is quite mature (see details below).
* Mechanical2.Common: this is a small library built on top of Mechanical2. It is quite new and small compared to it's big brother. It will provide tools which are slightly more specific, but still often useful.


Main Mechanical2 namespaces
---------------------------

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
The Mechanical twist on IoC containers. It has basic functionality, but is very extendable.
* Very simple, and strongly typed interface (2 methods only: Pull<T> and IsRegistered<T>)
* Designed to be easily integrated with other IoC containers.
* Immutable container: if you registered it, it's there. Based on how and what you register, this may provide thread-safety.
* Container hierarchy: write less code! Blacklist, whitelist, or simply replace mappings.
* Constructor, Property, Field and Method injection is possible.
* Mixing custom and injected parameters is also possible.
* Mappings for types like Func<T>, Lazy<T>, IEnumerable<T> and others, as well as your own, can be automatically generated.

### DataStores
Manual serialization with perks:
* Strongly typed, and hierarchical.
* High level of abstraction, but can manipulate low level data.
* Takes the best parts of common .NET classes: The ease of use from XElement, BinaryReader/Writer, and speed from the latter, as well as XmlReader/Writer.
* Hides the raw data format (text or binary), the file format (xml, json, ...), and optionally (through a MagicBag) the data representation (think ISO8601 for DateTime, cultures for numbers, ... etc.)
* Through the use of a MagicBag, changing the DateTime format is as easy as specifying a new Mapping. This can be done without changing a single line of serialization code anywhere (say in libraries, using standard data store serialization).

### Collections
Base classes for implementing your own collections. Make your code more expressive (think: Car.Wheels.Add...), or make generated values more accessible (than IEnumerable).

### IO
Replaces the functionality of StreamReader/Writer and BinaryReader/Writer.
* Interfaces abstract the handling of raw binary or textual data.
* Lightweight and reusable implementations (IDisposable is not required)
* Text readers and writers support Substring, for optimum performance (though this does require the use of special overloads).

### IO.FileSystem
Abstract interfaces for the most common file system functionality.
* Useful when writing cross-platform library code.
* File (or directory) paths returned and used by such an interface, are converted into a valid, portable and platform independent data store path.
* Only the most common operations are currently supported.
* Code is available to check file names for cross-platform portability.

### Events
Event Queue pattern with a twist (also known as event aggregator, message queue, ... etc.)
* Strongly typed: for example you can register for all events implementing a specific interface
* You can enqueue events in a fire & forget manner, similar to the usual implementation...
* ... but through tasks you can also be notified when it was handled, and if you wish, you may even intercept any exceptions thrown.
* This is similar to the functionality of standard .NET events.
* Event handlers can also "piggyback" events unto the one they are currently handling. This results in the event not being handled, until all piggybacked events get handled. (Just like .NET events triggering each other.)
* Events are processed in a single, long-running task.
* A two-stage shutdown process allows all subscribers plenty of opportunities to clean up after themselves.

### Log
A basic logging implementation.
* Simple but effective interface. A subset of NLog, but has no dependency on it.
* Designed to be simple to replace with your own logger, should you choose to do so.
* Log entries of the default implementation can be serialized using the data store.
* Exceptions can be serialized, and all major information can be restored, even on different platforms, which do not have the exception type.

### FileFormats
Low level, performance oriented parsers and writers for common file formats. Currently supported formats are:
* CSV
* JSON

### MVVM
Few minor tools for UI development. Useful for quickly developing small apps.
* INotifyPropertyChanged implementation using the new(ish) caller info attribute.
* UI thread handling. (Helps writing platform independent view model libraries).
* A simple delegate based ICommand implementation.
* A simple base class for quickly implementing IValueConverter using templates.
