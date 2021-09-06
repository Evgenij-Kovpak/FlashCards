using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.Model
{
    using DAL.Enities;
    class FlipCardWithKnowledge : ITrainData
    {
        public FlipCard FlipCard { get; set; }
        public sbyte Knowledge { get; set; }

        public FlipCardWithKnowledge(FlipCard flipCard, sbyte knowledge)
        {
            FlipCard = flipCard;
            Knowledge = knowledge;
        }
    }
}