﻿using System;

internal class InternalClass
{
    
    public InternalClass ()
    {
        
    }
    internal InternalClass(string @internal)
    {
        
    }
    private InternalClass(int @private)
    {
        
    }
    public InternalClass(bool @public)
    {
        
    }
    string PrivateField;
    internal string InternalField;
    public string PublicField;
    string PrivateProperty { get; set; }
    internal string InternalProperty { get; set; }
    public string PublicProperty { get; set; }
    event EventHandler PrivateEvent;
    internal event EventHandler InternalEvent;
    public event EventHandler PublicEvent; 
	void PrivateMethod()
	{
	}
	internal void InternalMethod()
	{
	}
	public void PublicMethod()
	{
	}
}