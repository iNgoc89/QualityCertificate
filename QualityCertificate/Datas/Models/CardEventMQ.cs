namespace QualityCertificate.Datas.Models
{
    public class CardEventMQ
    {
        public int CardId { get; set; }
        public int ReaderId { get; set; }
        public int? ReaderIndex { get; set; }
        public DateTime? LastDetectDate { get; set; }
        public long CardNumber { get; set; }
        /// <summary>
        /// Giá trị hexa của thẻ, chưa giải mã
        /// </summary>
        public string HexCardValue { get; set; }
        public int LocationId { get; set; }
    }
}
