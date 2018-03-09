﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IABParam
{
    string Identifier { get; }

	IABParam Clone ();

	System.Type getOutcomeType ();
}
