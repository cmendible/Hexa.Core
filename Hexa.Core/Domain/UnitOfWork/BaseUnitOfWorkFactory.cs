//----------------------------------------------------------------------------------------------
// <copyright file="BaseUnitOfWorkFactory.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    public abstract class BaseUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Current
        {
            get
            {
                return ContextState.Get<IUnitOfWork>("yada");
            }
            private set
            {
                ContextState.Store("yada", value);
            }
        }

        public IUnitOfWork Create(UnitOfWorkOption unitOfWorkOption = UnitOfWorkOption.NewOrReuse)
        {
            IUnitOfWork previousUnitOfWork = this.Current;

            if (unitOfWorkOption == UnitOfWorkOption.NewOrReuse && previousUnitOfWork != null)
            {
                return previousUnitOfWork;
            }

            INestableUnitOfWork newUnitOfWork = InternalCreate(previousUnitOfWork);
            this.Current = newUnitOfWork;
            return newUnitOfWork;
        }

        public void UpdateCurrent(INestableUnitOfWork unitOfWork)
        {
            this.Current = unitOfWork.Previous;
        }

        protected abstract INestableUnitOfWork InternalCreate(IUnitOfWork previousUnitOfWork);
    }
}
