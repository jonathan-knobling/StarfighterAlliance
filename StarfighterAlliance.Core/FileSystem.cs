using System.IO.Abstractions;

namespace StarfighterAlliance.Core;

public class FileSystem : System.IO.Abstractions.FileSystem
{
	private readonly string basePath;

	public FileSystem()
	{
		var directory = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());

		// Go up in directories until in directory with sln file
		while (directory != null && !directory.GetFiles(searchPattern: "*.sln").Any())
		{
			directory = directory.Parent;
		}

		basePath = directory?.FullName ??
				   throw new InvalidOperationException("Solution root not found in " + this.GetType().Name + "::ctor");
	}

	public override IFile File => new FileWrapper(this, basePath);

	private class FileWrapper : System.IO.Abstractions.FileWrapper
	{
		private readonly string basePath;

		public FileWrapper(IFileSystem fileSystem, string basePath) : base(fileSystem)
		{
			this.basePath = basePath;
		}

		public override string ReadAllText(string path)
		{
			string fullPath = System.IO.Path.IsPathFullyQualified(path) ? path : System.IO.Path.Combine(basePath, path);

			return base.ReadAllText(fullPath);
		}
	}
}