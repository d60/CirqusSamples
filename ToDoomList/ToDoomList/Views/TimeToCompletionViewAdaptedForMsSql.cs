using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using d60.Cirqus.Extensions;
using d60.Cirqus.MsSql.Views;
using d60.Cirqus.Views.ViewManagers;
using d60.Cirqus.Views.ViewManagers.Locators;
using ToDoomList.Events;

namespace ToDoomList.Views
{
    /// <summary>
    /// Unfortunately, the <see cref="MsSqlViewManager{TView}"/> is not capable of saving dictionaries - but it
    /// IS capable of saving a <see cref="HashSet{T}"/> of <see cref="System.String"/>, so we just store the
    /// dictionaries as key-value pairs
    /// </summary>
    public class TimeToCompletionViewAdaptedForMsSql : IViewInstance<InstancePerAggregateRootLocator>,
        ISubscribeTo<BulletAdded>,
        ISubscribeTo<BulletMarkedAsComplete>
    {
        public TimeToCompletionViewAdaptedForMsSql()
        {
            BulletAddedTimes = new HashSet<string>();
            CompletionTimes = new HashSet<string>();
        }
        
        public string Id { get; set; }
        
        public long LastGlobalSequenceNumber { get; set; }

        public HashSet<string> BulletAddedTimes { get; set; }

        public HashSet<string> CompletionTimes { get; set; }
        
        public void Handle(IViewContext context, BulletAdded domainEvent)
        {
            var key = domainEvent.Text;
            var timeWhenBulletWasAdded = domainEvent.GetUtcTime();

            var value = timeWhenBulletWasAdded.ToString("u");
            BulletAddedTimes.Add(string.Format("{0}={1}", key, value));
        }

        public void Handle(IViewContext context, BulletMarkedAsComplete domainEvent)
        {
            var toDoomList = context.Load<AggregateRoots.ToDoomList>(domainEvent.GetAggregateRootId());
            var text = toDoomList.Bullets[domainEvent.BulletIndex].Text;

            var key = text;
            
            var timeWhenBulletWasAdded = GetDateTimeForKey(key);
            var timeWhenBulletWasClosed = domainEvent.GetUtcTime();

            var timeToCompletion = timeWhenBulletWasClosed.ToUniversalTime()
                                   -timeWhenBulletWasAdded.ToUniversalTime();

            CompletionTimes.Add(string.Format("{0}={1}", key, timeToCompletion.ToString("g")));
        }

        DateTime GetDateTimeForKey(string key)
        {
            var dictionary = BulletAddedTimes
                .Select(str => str.Split('='))
                .Where(tokens => tokens.Length == 2)
                .ToDictionary(tokens => tokens[0], tokens => tokens[1]);

            return DateTime.ParseExact(dictionary[key], "u", CultureInfo.CurrentCulture);
        }
    }
}