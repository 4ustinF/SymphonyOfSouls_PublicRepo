using System.Collections.Generic;

public interface  IInteractable
{
    public int InteractionsRequired { get; set; }
    public void Interact(out List<RewardDef> _rewards);
}
