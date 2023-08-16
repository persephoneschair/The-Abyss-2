using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColumnManager : MonoBehaviour
{
    #region Init

    public static ColumnManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public Column[] columns;

    public void BringInPlayers()
    {
        StartCoroutine(ActivateColumns());
    }

    IEnumerator ActivateColumns()
    {
        List<Column> emptyColumns = columns.Where(x => x.containedPlayer == null).ToList();
        foreach(Column c in emptyColumns)
        {
            c.ActivateColumn();
            yield return new WaitForSeconds(5f);
            if (PlayerManager.Get.players.Count(x => !x.inHotseat) == 0)
                break;
        }
    }
}
