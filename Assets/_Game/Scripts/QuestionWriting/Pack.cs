using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Pack
{
    public string author;
    public List<Question> cat1Qs = new List<Question>();
    public List<Question> cat2Qs = new List<Question>();
    public List<Question> cat3Qs = new List<Question>();
    public List<Question> cat4Qs = new List<Question>();
    public List<Question> cat5Qs = new List<Question>();
}
