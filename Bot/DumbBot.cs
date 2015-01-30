using System;
using Engine;
using Models.Map;

namespace Bot
{
    public class DumbBot : AbstractBot
    {
        public DumbBot(IClient client) : base(client)
        {
            
        }
        
        public override void onMapInitialization(object sender, MapUpdateEventArgs mapInitializationEventArgs)
        {
            

        }
        public override void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventsArgs)
        {


        }
        public override void onHomeSet(object sender, MapUpdateEventArgs homeSetEventArgs)
        {


        }
        public override void onHousesSet(object sender, MapUpdateEventArgs housesSetEventArgs)
        {


        }
        public override void onGameEnd(object sender, EventArgs eventArgs)
        {


        }
        public override void onDisconnection(object sender, EventArgs eventArgs)
        {


        }

    }
}
