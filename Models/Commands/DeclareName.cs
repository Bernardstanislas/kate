namespace Models.Commands
{
	public class DeclareName: ICommand
	{
        public string Name { get; private set; }

		public DeclareName(string name)
		{
            this.Name = name;
		}
	}
}

