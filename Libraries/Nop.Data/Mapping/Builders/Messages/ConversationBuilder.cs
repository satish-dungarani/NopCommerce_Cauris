using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Messages
{
    public class ConversationBuilder : NopEntityBuilder<Conversation>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Conversation.FirstSenderId)).AsInt32().NotNullable()
                .WithColumn(nameof(Conversation.SecondSenderId)).AsInt32().NotNullable()
                .WithColumn(nameof(Conversation.CreationDate)).AsDateTime().NotNullable()
                .WithColumn(nameof(Conversation.LastMessageDate)).AsDateTime().Nullable();
        }
    }
}
