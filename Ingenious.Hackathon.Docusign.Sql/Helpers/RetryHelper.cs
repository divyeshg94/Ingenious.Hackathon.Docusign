namespace Ingenious.Hackathon.Docusign.Sql.Helpers
{
    public static class RetryHelper
    {
        public static async Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation,int maxRetries = 3)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return await operation(); // Execute the passed function
                }
                catch (ObjectDisposedException ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        throw new Exception("Max retries exceeded for accessing the DbContext.", ex);
                    }

                    // Optionally, add a delay between retries
                    await Task.Delay(1000);
                }
            }
        }

        public static async Task RetryOnExceptionAsync(Func<Task> operation, int maxRetries = 3)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    await operation(); // Execute the passed function
                    return;
                }
                catch (ObjectDisposedException ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        throw new Exception("Max retries exceeded for accessing the DbContext.", ex);
                    }

                    // Optionally, add a delay between retries
                    await Task.Delay(1000);
                }
            }
        }
    }
}
