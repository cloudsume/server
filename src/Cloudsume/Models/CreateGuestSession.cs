namespace Cloudsume.Models;

using System.ComponentModel.DataAnnotations;

public sealed record CreateGuestSession([MinLength(1)] string Captcha);
