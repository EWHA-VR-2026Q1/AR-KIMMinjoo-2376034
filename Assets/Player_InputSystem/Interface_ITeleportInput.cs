// 3. 텔레포트 시스템 관련
public interface ITeleportInput
{
    bool TeleportAimingInput { get; }
    bool TeleportExecuteInput { get; }
    bool TeleportCancelInput { get; }
}