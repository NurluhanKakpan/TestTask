using System;

namespace Antiban
{
    public class AntibanResult
    {
        /// <summary>
        /// Предполагаемое время отправки сообщения
        /// </summary>
        public DateTime SentDateTime { get; set; }
        public int EventMessageId { get; set; }

        public AntibanResult(DateTime sentDateTime, int eventMessageId)
        {
            EventMessageId = eventMessageId;
            SentDateTime = sentDateTime;
        }
    }
}
