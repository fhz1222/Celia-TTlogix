namespace Infrastructure.Email;

internal static class EmailRepository
{
    internal static string Regards
        => "<p>Best regards,</p><p>Toyota Tsusho Europe SA team</p>";

    internal static string NoReplyInfo 
        => "<p><small>** This is an auto-generated email. <u>Please do not reply to this message</u>. For enquiries, contact Toyota Tsusho Europe SA team.</small></p>";

    internal static string GetVmiWebUrl(string ownerCode)
        => ownerCode switch
        {
            "TESAP" => "https://pl.vmi.ttesa.net/Electrolux/web/#/invoicing",
            "TESAI" => "https://it.vmi.ttesa.net/Electrolux/web/#/invoicing",
            "TESAH" => "https://hu.vmi.ttesa.net/Electrolux/web/#/invoicing",
            "TESAG" => "https://de.vmi.ttesa.net/Electrolux/web/#/invoicing",
            _ => "_blank"
        };
}
