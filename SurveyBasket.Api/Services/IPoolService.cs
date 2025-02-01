namespace SurveyBasket.Api.Services;

public interface IPoolService
{
    IEnumerable<Poll> GetAll();
    Poll? Get(int id);
    Poll Add(Poll poll);
    
    bool Update(int id, Poll poll);
    bool Delete(int id);
}