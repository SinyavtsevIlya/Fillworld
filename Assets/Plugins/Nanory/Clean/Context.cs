using System;
using System.Collections.Generic;

namespace CleanRx
{
    public class Context
    {
        public List<Group> Groups;

        Entity[] _entities;
        int _entitiesCount;
        int[] _reservedEntities;
        int _reservedEntitiesCount;
        internal Dictionary<int, List<Group>> _groupsByData;

        List<Group> _groupsLookupCache;

        List<Group> _groupsRemovePool = new List<Group>();

        public Context()
        {
            _entities = new Entity[64];
            _reservedEntities = new int[64];
            _groupsByData = new Dictionary<int, List<Group>>();
            Groups = new List<Group>();
        }

        public Group CreateOrGetGroup(Group group)
        {
            foreach (var g in Groups)
            {
                if (group.AreEquals(g)) return g;
            }

            foreach (var dataIndex in group.WithTypeIndecies)
            {
                if (!_groupsByData.ContainsKey(dataIndex))
                {
                    var groups = new List<Group>();
                    groups.Add(group);
                    _groupsByData[dataIndex] = groups;
                }
                else
                {
                    _groupsByData[dataIndex].Add(group);
                }
            }
            Groups.Add(group);
            return group;
        }

        public Entity CreateEntity()
        {
            int id;
            if (_reservedEntitiesCount > 0)
            {
                _reservedEntitiesCount--;
                id = _reservedEntities[_reservedEntitiesCount];
            }
            else
            {
                id = _entitiesCount;
            }
            _entitiesCount++;

            var entity = new Entity(this);

            if (id == _entities.Length)
            {
                Array.Resize(ref _entities, _entitiesCount << 1);
            }

            _entities[id] = entity;

            entity.ID = id;
            entity.IsReserved = false;

            return entity;
        }

        public void Destroy(int id)
        {
            if (_entitiesCount == 0) return;
            Destroy(_entities[id]);
        }

        public void Destroy(Entity entity)
        {
            _entitiesCount--;

            entity.OnDestroy();
            entity.OnDestroy = () => { };

            if (entity.IsReserved) throw new System.Exception("Unable to destroy reserved entity " + entity.ID);

            entity.RemoveAll();

            entity.IsReserved = true;

            _reservedEntities[_reservedEntitiesCount++] = entity.ID;

            if (_reservedEntitiesCount == _reservedEntities.Length)
            {
                Array.Resize(ref _reservedEntities, _reservedEntitiesCount << 1);
            }

            entity.Generation++;
        }

        public Entity GetEntity(int id)
        {
            if (id < 0) return null;
            var entity = _entities[id];
            if (entity.IsReserved) return null;
            return entity;
        }

        public Group GetGroup() => new Group(this);

        internal void FilterEntityOnDataChange(Entity entity, int dataIndex, bool isAdded)
        {
            foreach (var group in Groups)
            {
                if (group.IsMatching(entity))
                {
                    //UnityEngine.Debug.Log($"{entity} is matching {group}");
                    entity.Groups.Add(group);
                    group.Entities.Add(entity);
                    group.ObserveCountChange.OnNext(entity);
                    group.ObserveAdd.OnNext(entity);
                } 
            }

            foreach (var group in entity.Groups)
            {
                if (!group.IsMatching(entity))
                {
                    _groupsRemovePool.Add(group);
                }
            }

            foreach (var group in _groupsRemovePool)
            {
                entity.Groups.Remove(group);
                group.ObserveCountChange.OnNext(entity);
                group.ObserveRemove.OnNext(entity);
                group.Entities.Remove(entity);
            }
            _groupsRemovePool.Clear();
        }

        public void FilterEntityOnCreateOrDestroy(Entity entity, bool isCreated)
        {
            foreach (var group in Groups)
            {
                if (isCreated)
                {
                    if (group.IsMatching(entity))
                    {
                        group.Entities.Add(entity);
                        group.ObserveAdd.OnNext(entity);
                        group.ObserveCountChange.OnNext(entity);
                        entity.Groups.Add(group);
                    }
                }
                else
                {
                    group.ObserveCountChange.OnNext(entity);
                    foreach (var entityGroup in entity.Groups)
                    {
                        entityGroup.Entities.Remove(entity);
                    }
                    entity.Groups.Clear();
                    group.ObserveReset.OnNext(UniRx.Unit.Default);
                }
            }
        }
    }
}
