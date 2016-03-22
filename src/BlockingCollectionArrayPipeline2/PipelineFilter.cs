using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace BlockingCollectionArrayPipeline2
{
    internal class PipelineFilter<TInput, TOutput>
    {
        public BlockingCollection<TInput>[] m_input;
        public BlockingCollection<TOutput>[] m_output = null;
            
        Func<TInput, TOutput> m_processor = null;
        Action<TInput> m_outputProcessor = null;
            
        public string Name { get; private set; }
        CancellationToken m_token;

        public PipelineFilter(
            BlockingCollection<TInput>[] input,
            Func<TInput, TOutput> processor,
            CancellationToken token,
            string name)
        {
            m_input = input;
            m_output = new BlockingCollection<TOutput>[5];
            for (int i = 0; i < m_output.Length; i++)
                m_output[i] = new BlockingCollection<TOutput>(500);

            m_processor = processor;
            m_token = token;
            Name = name;
        }

        // Use this constructor for the final endpoint, which does
        // something like write to file or screen, instead of
        // pushing to another pipeline filter.
        public PipelineFilter(
            BlockingCollection<TInput>[] input,
            Action<TInput> renderer,
            CancellationToken token,
            string name)
        {
            m_input = input;
            m_outputProcessor = renderer;
            m_token = token;
            Name = name;
        }

        public void Run()
        {
            Console.WriteLine("{0} is running", this.Name);
            while (!m_input.All(bc => bc.IsCompleted) && !m_token.IsCancellationRequested)
            {
                TInput receivedItem;
                int i = BlockingCollection<TInput>.TryTakeFromAny(
                    m_input, out receivedItem, 50, m_token);
                if (i >= 0)
                {
                    if (m_output != null) // we pass data to another blocking collection
                    {
                        TOutput outputItem = m_processor(receivedItem);
                        BlockingCollection<TOutput>.AddToAny(m_output, outputItem);
                        Console.WriteLine("{0} sent {1} to next", this.Name, outputItem);
                    }
                    else // we're an endpoint
                    {
                        m_outputProcessor(receivedItem);
                    }
                }
                else
                    Console.WriteLine("Unable to retrieve data from previous filter");
            }
            if (m_output != null)
            {
                foreach (var bc in m_output) bc.CompleteAdding();
            }
        }
    }
}