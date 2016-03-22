#Description

The **Path Structure Maintenance** application can be used to ensure a specific folder/file structure is implemented and maintained. Functions include:

 - Auditing the *default path*, *specified path*, and a *specified file* for a valid Path Structure
 - Formatting a specified file per the Path Structure
 - Creating a clipboard entry of the proper filename syntax for a specified Path Structure file type
 - Moving files by file type to a specified Path Structure folder.

#How to use

##Creating the Format Structure

First, create a **Path Structure** format file in XML using the following rules:

 - The root node must be `<Structure>`
 ..- `<Structure>` must contain a `defaultPath` attribute specifying the default root path that the Path Structure should focus on.
 ..- `<Structure>` must contain a `path` attribute specifying the syntax of the direct descendant folder names of `defaultPath`
 - Optionally, `<Variables>` may be added to the root.
 ..- `<Variable>` nodes can be added to `<Variables>` to allow the application to parse the folder/file path to extract the specified index and re-use the value.
 ..- `<Variable>` must contain a `name` attribute specifying the replacement reference of the variable. Note that the reference name must be enclosed in `{}`.
 ..- The inner text of `<Variable>` must be the zero-based index of the folder name to be extracted based on `defaultPath` of `<Structure>`
 ..- For example: `<Variable name='{CustomerName}'>2</Variable>` will set the variable `{CustomerName}` to `tbm0115` with the given path of `\\server\Customers\tbm0115\`
 - `<Folder>` may be added to both the root and other `<Folder>` nodes
 ..- `<Folder>` must contain a `name` attribute specifying the short description of the folder.
 ..- `name` may reference a `<Variable>` by its `name`.
 ..- `<Folder>` must contain a `description` attribute specifying the long description of the folder.
 - `<File>` may be added to both the root and `<Folder>` nodes
 ..- `<File>` must contain a `name` attribute specifying the short description of the file.
 ..- `name` may reference a `<Variable>` by its `name`.
 ..- `<File>` must contain a `description` attribute specifying the long description of the file.
 ..- Optionally, `<Option>` may be added to a `<File>` to provide optional format(s) for the `<File>`.
 ..- An `<Option>` node requires the same format as a `<File>` node.
 ..- If no `<Option>`'s are necessary, then the inner text of `<File>` must provide the expected file name format of the `<File>`.
 ..- For example: `<File name='Purchase Order'>{CustomerName}_PO</File>` in the folder `\\server\Customers\tbm0115\` will require the `Purchase Order` type file to have the filename of `tbm0115_PO` with whichever file extension is necessary.