namespace EriccoreTech.Integration.Persistence {
    public sealed class WorkflowRecord {

        public string Id { get; set; } = "";

        public int Version { get; set; }

        public string DefinitionJson { get; set; } = "{}";

        public bool Published { get; set; }

    }
}
