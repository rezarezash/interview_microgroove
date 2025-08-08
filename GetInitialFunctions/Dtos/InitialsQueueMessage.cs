namespace GetInitialFunctions.Dtos
{
    public sealed class InitialsQueueMessageDto
    {
        public required string FullName { get; set; }
        public int Id { get; set; }

        public static InitialsQueueMessageDto Create(string firstName, string lastName, int id)
                => new() { FullName = $"{firstName} {lastName}", Id = id };
    }
}
