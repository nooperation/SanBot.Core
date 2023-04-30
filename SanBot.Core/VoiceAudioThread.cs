using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanBot.Core
{
    public class VoiceAudioThread
    {
        private Action<byte[]> Callback { get; set; }
        private ConcurrentQueue<List<byte[]>> AudioDataQueue { get; set; } = new ConcurrentQueue<List<byte[]>>();
        private Thread ConsumerThread { get; set; }

        private volatile bool _isRunning = false;
        public DateTime LastTimeSomeoneSpoke { get; set; }
        public bool TryToAvoidInterruptingPeople { get; set; }

        private volatile bool _isSpeaking = false;
        public bool IsSpeaking => _isSpeaking;

        public VoiceAudioThread(Action<byte[]> callback, bool tryToAvoidInterruptingPeople)
        {
            Callback = callback;

            ConsumerThread = new Thread(new ThreadStart(Consumer));
            TryToAvoidInterruptingPeople = tryToAvoidInterruptingPeople;
        }

        public void Start()
        {
            _isRunning = true;
            ConsumerThread.Start();
        }
        public void Stop()
        {
            _isRunning = false;
            ConsumerThread.Join();
        }

        public void EnqueueData(List<byte[]> audioData)
        {
            AudioDataQueue.Enqueue(audioData);
        }

        public void Consumer()
        {
            long previousTickCount = 0;

            while (_isRunning)
            {
                if ((DateTime.Now - LastTimeSomeoneSpoke).TotalSeconds < 1)
                {
                    Thread.Yield();
                    continue;
                }

                if (AudioDataQueue.TryDequeue(out List<byte[]>? rawAudioPackets))
                {
                    Console.WriteLine($"VoiceAudioThread: Audio payload found. Sending it... rawAudioPackets={rawAudioPackets.Count}");
                    _isSpeaking = true;

                    for (int i = 0; i < rawAudioPackets.Count; i++)
                    {
                        Callback(rawAudioPackets[i]);

                        if (TryToAvoidInterruptingPeople && (DateTime.Now - LastTimeSomeoneSpoke).TotalSeconds < 1)
                        {
                            // This is not safe in any safe in any sort of manner :D
                            Console.WriteLine($"We are being interrupted. Pausing speech for now");
                            var newQueue = new ConcurrentQueue<List<byte[]>>();

                            // NOTE: We cannot skip back to previously played samples because each sample has the
                            //       sequence id already baked into it. We'd need to construct the packets here
                            //       instead of elsewhere, which I don't really want to do.
                            newQueue.Enqueue(rawAudioPackets.Skip(i).ToList());
                            foreach (var item in AudioDataQueue)
                            {
                                newQueue.Enqueue(item);
                            }
                            AudioDataQueue = newQueue;
                            break;
                        }

                        while ((DateTimeOffset.Now.Ticks - previousTickCount) < 200000)
                        {
                            // nothing
                        }
                        previousTickCount = DateTimeOffset.Now.Ticks;
                    }

                    _isSpeaking = false;
                }
                else
                {
                    Thread.Yield();
                }
            }
        }
    }
}
