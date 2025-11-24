// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.DataAccess.Domains.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }

    public string CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string LastModifiedBy { get; set; }
}
