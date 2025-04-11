namespace TextShare.Domain.Entities.Complaints
{
    /// <summary>
    /// Класс причины жалобы.
    /// </summary>
    public class ComplaintReasons
    {
        public int ComplaintReasonsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        public override string ToString()
        {
            return $"Id: {ComplaintReasonsId.ToString()}. Name {Name}.";
        }
    }
}
