# Checksum Calculator: A Design Patterns Project

This document provides a detailed overview of the "ChecksumCalculator" C# project, created to demonstrate the practical application of software design patterns. It details the project's structure, features, and the key patterns that form its architectural foundation.

---

## 1. Project Overview

Checksum Calculator is a .NET console application designed to compute cryptographic checksums (hashes) for files within a specified directory or for a single file. The application is built with extensibility and maintainability in mind, leveraging several design patterns to create a decoupled and flexible architecture.

### Core Features
* **Multiple Hashing Algorithms**: Supports standard algorithms like MD5, SHA1, SHA256, SHA384, and SHA512. The design allows for new algorithms to be added without changing existing code.
* **Recursive Directory Traversal**: Can process all files within a directory and its subdirectories.
* **Symbolic Link Handling**: Provides an option to either follow or ignore symbolic links during directory traversal.
* **Command-Line Interface**: Accepts arguments for path, algorithm, and symlink behavior.

---

## 2. Project Structure

The solution is organized into logical projects and namespaces to separate concerns:

* **`ChecksumCalculator`**: The main executable project.
    * `calculators`: Contains the `IChecksumCalculator` interface, its concrete implementations for each hash algorithm, and the `ChecksumCalculatorFactory`.
    * `fs`: Contains classes for representing and processing the file system. 
    * `Program.cs`: The application's entry point, which parses command-line arguments and orchestrates the application's components.
* **`ChecksumCalculator.Tests`**: A separate project containing a comprehensive suite of xUnit tests to ensure the correctness and robustness of the core logic.
    * `resources`: Contains various files and directories used for testing.


---

## 3. Design Patterns in Use

The project's architecture heavily relies on several fundamental design patterns to achieve its goals of flexibility, decoupling, and maintainability.

### Factory Method

The **Factory Method** pattern provides an interface for creating objects in a superclass but allows subclasses to alter the type of objects that will be created. Here, we use a static factory variation to decouple the client from the concrete implementation of the checksum calculators.

* **Implementation**: The `ChecksumCalculatorFactory` class provides a static `Create(string algorithm)` method. It uses reflection to dynamically find all classes that implement `IChecksumCalculator` and instantiates the one that matches the requested algorithm name.

    ```csharp
    // In calculators/ChecksumCalculatorFactory.cs
    public class ChecksumCalculatorFactory
    {
        public static IChecksumCalculator Create(string algorithm)
        {
            // Find the type that matches the algorithm name using reflection
            var calculatorType = calculatorTypes.FirstOrDefault(type => 
                type.Name.Equals(algorithm + "ChecksumCalculator", StringComparison.OrdinalIgnoreCase));
            
            if (calculatorType == null) { /*...*/ }

            // Create an instance of the found type
            var instance = Activator.CreateInstance(calculatorType) as IChecksumCalculator;
            //...
            return instance;
        }
    }
    ```

* **Purpose**: This pattern **decouples the client** (`Program.cs`) from the concrete calculator classes (`MD5ChecksumCalculator`, `SHA1ChecksumCalculator`, etc.). The client doesn't need to know how to construct a specific calculator; it just requests one from the factory. This makes the system extremely extensible: adding a new hashing algorithm only requires creating a new class that implements `IChecksumCalculator`, with **no changes needed** in the factory or the client code.

### Strategy

The **Strategy** pattern defines a family of algorithms, encapsulates each one, and makes them interchangeable. This lets the algorithm vary independently from the clients that use it.

* **Implementation**:
    1.  `IChecksumCalculator` is the **Strategy** interface, which defines a common `Calculate` method for all supported algorithms.
    2.  `MD5ChecksumCalculator`, `SHA1ChecksumCalculator`, etc., are the **Concrete Strategies**. Each class provides a specific implementation of a hashing algorithm.
    3.  The `HashStreamWriter` class acts as the **Context**, which is configured with a concrete strategy object at runtime and uses it to perform the calculation.

    ```csharp
    // The Strategy interface
    public interface IChecksumCalculator
    {
        string Calculate(Stream inputStream);
    }

    // The Context that uses the chosen strategy
    public class HashStreamWriter : FileProcessorVisitor
    {
        private readonly IChecksumCalculator calculator; // Holds the strategy object

        public HashStreamWriter(IChecksumCalculator calculator, TextWriter writer)
        {
            this.calculator = calculator;
            // ...
        }

        protected override void ProcessFile(FileNode node)
        {
            // ...
            // Delegates the calculation to the strategy object
            var hash = calculator.Calculate(stream); 
            writer.WriteLine($"{node.Path}: {hash}");
            // ...
        }
    }
    ```

* **Purpose**: This pattern **decouples the hashing logic** from the part of the application that processes files. The `HashStreamWriter` doesn't need to know the details of any specific algorithm. This makes it easy to add new hashing algorithms or change existing ones without affecting the file processing code, promoting the **Open/Closed Principle**.


### Composite

The **Composite** pattern is used to compose objects into tree structures to represent part-whole hierarchies. It lets clients treat individual objects and compositions of objects uniformly.

* **Implementation**:
    1.  The `FileSystemNode` is the abstract component that declares the interface for objects in the composition.
    2.  `FileNode` is a "leaf" object that represents a single file.
    3.  `DirectoryNode` is the "composite" object that can contain other `FileSystemNode`s (both `FileNode`s and other `DirectoryNode`s).

    ```csharp
    // In fs/FileSystemNode.cs (Component)
    public abstract class FileSystemNode
    {
        public string Path { get; }
        public abstract void Accept(FileSystemVisitor visitor);
    }

    // In fs/DirectoryNode.cs (Composite)
    public class DirectoryNode : FileSystemNode
    {
        public List<FileSystemNode> Children { get; private set; }

        public void AddChild(FileSystemNode child)
        {
            Children.Add(child);
            Size += child.Size;
        }
        // ...
    }
    ```
* **Purpose**: This pattern allows the file system tree to be processed uniformly. The `Visitor` (see below) can traverse the entire structure by calling `Accept()` on the root node, without needing to distinguish between files and directories in its traversal logic.

### Builder

The **Builder** pattern separates the construction of a complex object from its representation, allowing the same construction process to create different representations. This is useful when the object requires a multi-step or complex creation process.

* **Implementation**:
    1.  The `FileSystemBuilder` is the abstract Builder interface, defining the contract for building the complex object (the `FileSystemNode` tree).
    2.  `SymlinkIgnoringBuilder` and `SymlinkFollowingBuilder` are concrete Builder classes. Each implements the `Build` method with a different internal logic (strategy) for handling file system entries.
    3.  The client (`Program.cs`) acts as the Director, choosing the appropriate builder and initiating the construction process.

    ```csharp
    // In Program.cs (The Client/Director)
    FileSystemBuilder builder = followSymlinks
        ? new SymlinkFollowingBuilder()
        : new SymlinkIgnoringBuilder();

    var rootNode = builder.Build(path);
    ```

* **Purpose**: This pattern **encapsulates the complex logic** of constructing the file system tree. It isolates the client from the details of the construction, such as how directories are traversed or how symbolic links are resolved. This makes it easy to switch between different construction methods without changing the client code.


### Visitor

The **Visitor** pattern separates an algorithm from the object structure on which it operates. It allows you to add new operations to existing object structures without modifying those structures.

* **Implementation**:
    1.  The `FileSystemVisitor` abstract class declares `VisitFile` and `VisitDirectory` methods.
    2.  Each `FileSystemNode` subclass implements an `Accept` method that calls the appropriate method on the visitor (`visitor.VisitFile(this)` or `visitor.VisitDirectory(this)`).
    3.  `HashStreamWriter` and `ReportWriter` are concrete visitor implementations that perform different operations on the file nodes they visit.

    ```csharp
    // In fs/FileSystemVisitor.cs (Visitor Interface)
    public abstract class FileSystemVisitor
    {
        public abstract void VisitFile(FileNode fileNode);
        public abstract void VisitDirectory(DirectoryNode dir);
    }

    // In fs/HashStreamWriter.cs (Concrete Visitor)
    public class HashStreamWriter : FileProcessorVisitor
    {
        // ...
        protected override void ProcessFile(FileNode node)
        {
            using (var stream = File.OpenRead(node.Path))
            {
                var hash = calculator.Calculate(stream);
                writer.WriteLine($"{node.Path}: {hash}");
            }
        }
    }

    // In fs/FileNode.cs (Element)
    public class FileNode : FileSystemNode
    {
        public override void Accept(FileSystemVisitor visitor)
        {
            visitor.VisitFile(this);
        }
    }
    ```

* **Purpose**: The Visitor pattern makes the system highly extensible. We can add completely new functionalities (e.g., a visitor to find the largest file, a visitor to archive files) just by creating a new visitor class, without ever modifying the `FileNode` or `DirectoryNode` classes. This adheres to the **Open/Closed Principle**.


### Template Method

The **Template Method** pattern defines the skeleton of an algorithm in a base class but lets subclasses override specific steps of the algorithm without changing its overall structure.

* **Implementation**: The abstract `FileProcessorVisitor` class defines the "template method" for processing file system nodes. The `VisitDirectory` method provides a fixed algorithm for recursively traversing a directory. The crucial part is the `public sealed override void VisitFile(FileNode file)`, which forces subclasses to follow the template and delegates the specific action on a file to the `protected abstract void ProcessFile(FileNode file)` method.

    ```csharp
    // In fs/FileProcessorVisitor.cs (Abstract Class with Template)
    public abstract class FileProcessorVisitor : FileSystemVisitor
    {
        // This is part of the fixed algorithm (template)
        public sealed override void VisitFile(FileNode file)
        {
            ProcessFile(file);
        }

        // This is the variable step that subclasses must implement
        protected abstract void ProcessFile(FileNode file);
    }

    // In fs/ReportWriter.cs (Concrete Class)
    public class ReportWriter : FileProcessorVisitor
    {
        // ...
        protected override void ProcessFile(FileNode file)
        {
            // Provides the specific implementation for the variable step
            writer.WriteLine($"{Path.GetFileName(file.Path)}: {file.Size} bytes");
        }
    }
    ```

* **Purpose**: This pattern enforces a consistent structure for operations that traverse the file system tree while allowing for variation in what happens when a file is processed. It avoids code duplication by placing the common traversal logic in the base class. 

---

## 4. How to Use

The application is run from the command line.

### Options

* `--path <string>`: The full path to the file or directory to process. If not provided, it defaults to the current directory.
* `--algorithm <string>`: The hashing algorithm to use. Supported values are `MD5`, `SHA1`, `SHA256`, `SHA384`, `SHA512`. Defaults to `MD5`.
* `--follow-symlinks`: A flag that, if present, instructs the application to follow symbolic links. By default, they are ignored.

---

## 5. References

[Factory Method Pattern - Wikipedia](https://en.wikipedia.org/wiki/Factory_method_pattern)  
[Strategy Pattern - Wikipedia](https://en.wikipedia.org/wiki/Strategy_pattern)  
[Composite Pattern - Wikipedia](https://en.wikipedia.org/wiki/Composite_pattern)  
[Builder Pattern - Wikipedia](https://en.wikipedia.org/wiki/Builder_pattern)  
[Visitor Pattern - Wikipedia](https://en.wikipedia.org/wiki/Visitor_pattern)  
[Template Method Pattern - Wikipedia](https://en.wikipedia.org/wiki/Template_method_pattern)  
[.NET Parse the Command Line with System.CommandLine - Microsoft Docs](https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/march/net-parse-the-command-line-with-system-commandline)  
[System.Security.Cryptography Namespace - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography?view=net-8.0)  
[C# Get a Files Checksum Using Any Hashing Algorithm - Makolyte](https://makolyte.com/csharp-get-a-files-checksum-using-any-hashing-algorithm-md5-sha256/)  
[Online Hash Calculator - Tools4Noobs](https://www.tools4noobs.com/online_tools/hash/)  
[How to Enumerate Directories and Files - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-enumerate-directories-and-files)  
[Best Way to Iterate Folders and Subfolders - Stack Overflow](https://stackoverflow.com/questions/5181405/best-way-to-iterate-folders-and-subfolders)  
