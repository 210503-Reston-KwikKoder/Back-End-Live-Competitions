using System.Threading.Tasks;
using Octokit;

namespace LiveCompetitionBL
{
    public interface ISnippets
    {
        Task<TestMaterial> GetRandomQuote();
        Task<TestMaterial> GetCodeSnippet(int id);
        Task<string> GetAuth0String();
    }
}