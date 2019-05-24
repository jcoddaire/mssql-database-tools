# About
This is a tool that I created for automating some SQL tasks. This tool can:

1. Download SQL objects and save them as text files in a CI/CD ready way.
2. Compare SQL objects in two different databases and tell you what objects are different. This is useful if you forgot to migrate a procedure, view, or trigger when moving to prod.

# Supported Platforms
This works on any Microsoft SQL Server installation. Versions must be between 2008 - 2017. I haven't tested it on anything more recent, but I imagine it will work just fine.

I might be able to add Oracle support at some point. TBD.

# Usage
Right so only the SQL download tool is implemented right now. To use this, open the [App.config](./MSSQL-Database-Tools/App.config) file and update your connection strings. You can add new databases too.

Once that is done head over to the [Program.cs](./MSSQL-Database-Tools/Program.cs) file. Update the `databases` variable to match the keys in your `App.config`. If you want to change the `exportLocation`, do so here. This will be where all the text files are saved to.

Then just run it. I recommend running this against a dev database first, since running random code against a production database is generally a bad idea. But if you like living a risky life I'm not going to stop you :D

# Errors
If you do not have access to the database, you will get a nasty error. Read the stacktrace. I haven't added nice logging to this yet.

If you get other errors just shoot an email my way. I would be happy to help.