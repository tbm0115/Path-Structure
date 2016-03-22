#Description

The **Path Structure Maintenance** application can be used to ensure a specific folder/file structure is implemented and maintained. Functions include:

 * Auditing the *default path*, *specified path*, and a *specified file* for a valid Path Structure
 * Formatting a specified file per the Path Structure
 * Creating a clipboard entry of the proper filename syntax for a specified Path Structure file type
 * Moving files by file type to a specified Path Structure folder.

#How to use

##Creating the Format Structure

First, create a **Path Structure** format file in XML using the following rules:

 1. The root node must be `<Structure>`
 ··* `<Structure>` must contain a `defaultPath` attribute specifying the default root path that the Path Structure should focus on.
 ··* `<Structure>` must contain a `path` attribute specifying the syntax of the direct descendant folder names of `defaultPath`
 1. Optionally, `<Variables>` may be added to the root.
 ··* `<Variable>` nodes can be added to `<Variables>` to allow the application to parse the folder/file path to extract the specified index and ··*use the value.
 ··* `<Variable>` must contain a `name` attribute specifying the replacement reference of the variable. Note that the reference name must be enclosed in `{}`.
 ··* The inner text of `<Variable>` must be the ze··*based index of the folder name to be extracted based on `defaultPath` of `<Structure>`
 ··* For example: `<Variable name='{CustomerName}'>2</Variable>` will set the variable `{CustomerName}` to `tbm0115` with the given path of `\\server\Customers\tbm0115\`
 1. `<Folder>` may be added to both the root and other `<Folder>` nodes
 ··* `<Folder>` must contain a `name` attribute specifying the short description of the folder.
 ··* `name` may reference a `<Variable>` by its `name`.
 ··* `<Folder>` must contain a `description` attribute specifying the long description of the folder.
 1. `<File>` may be added to both the root and `<Folder>` nodes
 ··* `<File>` must contain a `name` attribute specifying the short description of the file.
 ··* `name` may reference a `<Variable>` by its `name`.
 ··* `<File>` must contain a `description` attribute specifying the long description of the file.
 ··* Optionally, `<Option>` may be added to a `<File>` to provide optional format(s) for the `<File>`.
 ··* An `<Option>` node requires the same format as a `<File>` node.
 ··* If no `<Option>`'s are necessary, then the inner text of `<File>` must provide the expected file name format of the `<File>`.
 ··* For example: `<File name='Purchase Order'>{CustomerName}_PO</File>` in the folder `\\server\Customers\tbm0115\` will require the `Purchase Order` type file to have the filename of `tbm0115_PO` with whichever file extension is necessary.

For example:
```
<Structure defaultPath="\\server\Customers" path="{CustomerName}">
	<Variables>
		<Variable name="{CustomerName}">2</Variable>
	</Variables>
	<Folder name="{CustomerName}" description="Contains pertinent customer information.">
		<File name="Purchase Order" description="A Purchase Order template">{CustomerName}_PO</File>
		<File name="Customer Requirements" description="A document containing the customers requirements">
			<Option name="Master" description="The master (or active) version of the customers requirements">{CustomerName}_Master Requirements</Option>
			<Option name="Revision" description="A deprecated version of the customer requirements">{CustomerName}_Rev-{RevNo}_Requirements</Option>
		</File>
	</Folder>
</Structure>
```

##Setting the application

In order to use the application properly

 1. Open the EXE in **Administrator Mode**
 ··* From the main form, navigate to the *Settings* menu item.
 1. Click **Browse.··** to specify the location of the Path Structure XML file.
 1. The program will require to restart to save the changes

##Windows Context Menu

The Path Structure application has the option to add specific functions to the Windows Explorer context menu. To do so:

 1. Open the EXE in **Administrator Mode**
 ··* From the main form, navigate to the *Settings* menu item.
 1. Check/Uncheck the context items you wish to use/not use on the current machine.
 1. If a context menu exists already, it is recommended to click the *Remove Windows Context Menu* button to ensure unwanted items are no longer available
 1. Click the *Add/Update Windows Context Menu* button to add all checked context items

With the above steps completed, the context menu should be immediately available under the context menu item of **Path Structure**.

Note that most commands from the context menu and the application itself will only work within the Path Structure `<Structure>` `defaultPath`.