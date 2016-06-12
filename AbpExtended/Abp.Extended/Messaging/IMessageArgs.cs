namespace Abp.Messaging
{
    public interface IMessageArgs
    {
        string AssemblyQualifiedName { get; set; }

        string Content { get; set; }
    }
}
