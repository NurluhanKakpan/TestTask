using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Antiban;

public class Antiban
{
    private List<EventMessage> List;
    private List<EventMessage> listClone;

    public Antiban()
    {
        List = new List<EventMessage>();
        listClone = new List<EventMessage>();
    }

    /// <summary>
    /// Добавление сообщений в систему, для обработки порядка сообщений
    /// </summary>
    /// <param name="eventMessage"></param>
    public void PushEventMessage(EventMessage eventMessage)
    {
        List.Add(eventMessage);
        listClone.Add(eventMessage);
    }

    /// <summary>
    /// Вовзращает порядок отправок сообщений
    /// </summary>
    /// <returns></returns>
    public List<AntibanResult> GetResult()
    {
        //Example
        var result = new List<AntibanResult>();
        for (var i = 0; i < List.Count; i++)
        {
            var listOfDictionaries = new List<Dictionary<string, string>>(); 
            if (result.Count == 0)
            {
                result.Add(new AntibanResult(List[i].DateTime,List[i].Id));
                continue;
            }
            
            for ( var j = 0; j < result.Count; j++ )
            {
                var itemFromResult = List.FirstOrDefault(q => q.Id == result[j].EventMessageId);
                listOfDictionaries.Add(new Dictionary<string, string>
                {
                    {"Number", itemFromResult!.Phone },
                    {"Priority",itemFromResult.Priority.ToString()},
                    {"Id",itemFromResult.Id.ToString()},
                    {"Date",itemFromResult.DateTime.ToString(CultureInfo.CurrentCulture)}
                });
            }

            var maxIdWherePhoneIsDifferent =
                listOfDictionaries.Where(q => q["Number"] != List[i].Phone).Max(q => int.Parse(q["Id"]));
            int? maxIdWherePhoneIsSame = null;
            for (int k = 0; k < listOfDictionaries.Count; k++)
            {
                if (listOfDictionaries[k].ContainsValue(List[i].Phone))
                {
                    maxIdWherePhoneIsSame = listOfDictionaries.Where(q => q["Number"] == List[i].Phone ).Max(q => int.Parse(q["Id"]));
                }
            }
            var itemFromListWherePhoneIsDiffer = List.Find(q => q.Id == maxIdWherePhoneIsDifferent);
            EventMessage? itemFromListWherePhoneIsSame = null;
            if (maxIdWherePhoneIsSame != null)
            {
                itemFromListWherePhoneIsSame = List.Find(q => q.Id == maxIdWherePhoneIsSame)!;
            }
            
            if (itemFromListWherePhoneIsSame != null && (int)((listClone[i].DateTime - itemFromListWherePhoneIsSame!.DateTime).TotalHours) < 24 && listClone[i].Priority == itemFromListWherePhoneIsSame.Priority && List[i].Priority == 1 && listClone[i].Phone == itemFromListWherePhoneIsSame.Phone    )
            {
                List[i].DateTime = itemFromListWherePhoneIsSame.DateTime.AddHours(24);
            }
            
            else if ( itemFromListWherePhoneIsSame != null && (int)((listClone[i].DateTime - itemFromListWherePhoneIsSame.DateTime).TotalMinutes) < 1 &&
                      listClone[i].Priority == itemFromListWherePhoneIsSame.Priority && listClone[i].Priority == 0 && listClone[i].Phone == itemFromListWherePhoneIsSame.Phone)
            {
                List[i].DateTime = itemFromListWherePhoneIsSame.DateTime.AddMinutes(1);
            }
            else if ( itemFromListWherePhoneIsSame != null && listClone[i].Phone == itemFromListWherePhoneIsSame.Phone &&
                     listClone[i].Priority != itemFromListWherePhoneIsSame.Priority &&
                     (int)((listClone[i].DateTime - itemFromListWherePhoneIsSame.DateTime).TotalMinutes) < 1 )
            {
                List[i].DateTime = itemFromListWherePhoneIsSame.DateTime.AddMinutes(1);
            }
            else if ( itemFromListWherePhoneIsDiffer != null && (int)(listClone[i].DateTime - itemFromListWherePhoneIsDiffer.DateTime).TotalSeconds < 10 &&
                      listClone[i].Phone != itemFromListWherePhoneIsDiffer.Phone )
            {
                List[i].DateTime = itemFromListWherePhoneIsDiffer.DateTime.AddSeconds(10);
            }
            
            result.Add(new AntibanResult(List[i].DateTime,List[i].Id));
        }

        return result.OrderBy(q=>q.SentDateTime).ToList();
    }
}
