namespace LoadingModule.Entity
{
    /// <summary>
    /// Used for Deep First Search. 
    /// </summary>
    internal enum GraphNodeState
    {
        NotChecked, //Indicates that this node has not been visited yet
        Prepared,   //Indicates that this node was visited, but not exited
        Ready,      //Indicates that this node was successfully checked
        Cycle       //Indicates that this node involve in cyclic dependency
    }
}