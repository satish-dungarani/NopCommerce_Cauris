using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Messages
{
   public class ConversationMessageBuilder : NopEntityBuilder<ConversationMessage>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ConversationMessage.SenderId)).AsInt32().NotNullable()
                .WithColumn(nameof(ConversationMessage.ReceiverId)).AsInt32().NotNullable()
                .WithColumn(nameof(ConversationMessage.ConversationId)).AsInt32().NotNullable()
                .WithColumn(nameof(ConversationMessage.IsRead)).AsBoolean().NotNullable()
                .WithColumn(nameof(ConversationMessage.CreationDate)).AsDateTime().NotNullable();
        }
    }
}
