namespace WorldActionSystem
{
    internal interface IPickUpAbleItem
    {
        void OnPickUp();
        void QuickMoveBack();
        void NormalMoveBack();
    }
}