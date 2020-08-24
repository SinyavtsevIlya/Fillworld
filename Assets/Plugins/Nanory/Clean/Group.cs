using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace CleanRx
{
    public class Group : IEnumerable<Entity>
    {
        public readonly HashSet<Entity> Entities = new HashSet<Entity>();
        public Subject<Entity> ObserveAdd = new Subject<Entity>();
        public Subject<Entity> ObserveCountChange = new Subject<Entity>();
        public Subject<Entity> ObserveRemove = new Subject<Entity>();
        public Subject<Unit> ObserveReset = new Subject<Unit>();
        public List<int> WithTypeIndecies = new List<int>();
        public List<int> WithoutTypeIndecies = new List<int>();
        internal readonly Context _context;

        public Group(Context context)
        {
            _context = context;
        }

        public int Count => Entities.Count;

        public IEnumerator<Entity> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Group With<T1>()
        {
            EcsTypesHelper.Register<T1>();
            WithTypeIndecies.Add(typeof(T1).GetHashCode());
            return _context.CreateOrGetGroup(this);
        }

        public Group With<T1, T2>()
        {
            EcsTypesHelper.Register<T1>();
            EcsTypesHelper.Register<T2>();
            WithTypeIndecies.Add(typeof(T1).GetHashCode());
            WithTypeIndecies.Add(typeof(T2).GetHashCode());
            return _context.CreateOrGetGroup(this);
        }

        public Group With<T1, T2, T3>()
        {
            EcsTypesHelper.Register<T1>();
            EcsTypesHelper.Register<T2>();
            EcsTypesHelper.Register<T3>();
            WithTypeIndecies.Add(typeof(T1).GetHashCode());
            WithTypeIndecies.Add(typeof(T2).GetHashCode());
            WithTypeIndecies.Add(typeof(T3).GetHashCode());
            return _context.CreateOrGetGroup(this);
        }

        public Group Without<T1>()
        {
            EcsTypesHelper.Register<T1>();
            var withoutGroup = new Group(_context);
            withoutGroup.WithTypeIndecies = new List<int>(WithTypeIndecies);
            withoutGroup.WithoutTypeIndecies.Add(typeof(T1).GetHashCode());
            return _context.CreateOrGetGroup(withoutGroup);
        }

        public Group Without<T1, T2>()
        {
            EcsTypesHelper.Register<T1>();
            EcsTypesHelper.Register<T2>();
            var withoutGroup = new Group(_context);
            withoutGroup.WithTypeIndecies = new List<int>(WithTypeIndecies);
            withoutGroup.WithoutTypeIndecies.Add(typeof(T1).GetHashCode());
            withoutGroup.WithoutTypeIndecies.Add(typeof(T2).GetHashCode());
            return _context.CreateOrGetGroup(withoutGroup);
        }

        public Group Without<T1, T2, T3>()
        {
            EcsTypesHelper.Register<T1>();
            EcsTypesHelper.Register<T2>();
            EcsTypesHelper.Register<T3>();
            var withoutGroup = new Group(_context);
            withoutGroup.WithTypeIndecies = new List<int>(WithTypeIndecies);
            withoutGroup.WithoutTypeIndecies.Add(typeof(T1).GetHashCode());
            withoutGroup.WithoutTypeIndecies.Add(typeof(T2).GetHashCode());
            withoutGroup.WithoutTypeIndecies.Add(typeof(T3).GetHashCode());
            return _context.CreateOrGetGroup(withoutGroup);
        }

        public Group Without<T1, T2, T3, T4>()
        {
            EcsTypesHelper.Register<T1>();
            EcsTypesHelper.Register<T2>();
            EcsTypesHelper.Register<T3>();
            EcsTypesHelper.Register<T4>();
            var withoutGroup = new Group(_context);
            withoutGroup.WithTypeIndecies = new List<int>(WithTypeIndecies);
            withoutGroup.WithoutTypeIndecies.Add(typeof(T1).GetHashCode());
            withoutGroup.WithoutTypeIndecies.Add(typeof(T2).GetHashCode());
            withoutGroup.WithoutTypeIndecies.Add(typeof(T3).GetHashCode());
            withoutGroup.WithoutTypeIndecies.Add(typeof(T4).GetHashCode());
            return _context.CreateOrGetGroup(withoutGroup);
        }

        public bool AreEquals(Group other)
        {
            if (WithTypeIndecies.Count != other.WithoutTypeIndecies.Count) return false;

            foreach (var withIndex in WithTypeIndecies)
            {
                if (!other.WithTypeIndecies.Contains(withIndex))
                {
                    return false;
                }
            }

            if (WithoutTypeIndecies.Count != other.WithoutTypeIndecies.Count) return false;

            foreach (var withoutIndex in WithoutTypeIndecies)
            {
                if (!other.WithoutTypeIndecies.Contains(withoutIndex))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsMatching(Entity entity)
        {
            foreach (var withIndex in WithTypeIndecies)
            {
                if (!entity.Has(withIndex))
                {
                    return false;
                }
            }

            foreach (var withoutIndex in WithoutTypeIndecies)
            {
                if (entity.Has(withoutIndex))
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            var withIndeces = "";
            foreach (var id in WithTypeIndecies)
            {
                withIndeces += " " + EcsTypesHelper.GetTypeByHashCode(id).Name;
            }
            withIndeces = $"< {withIndeces} >";
            var withoutIndeces = "";
            foreach (var id in WithoutTypeIndecies)
            {
                withoutIndeces += " " + EcsTypesHelper.GetTypeByHashCode(id).Name;
            }
            withoutIndeces = $"< {withoutIndeces} >";
            var result = $" Group with: <b> <color=green>  {withIndeces} </color> </b>";
            if (WithoutTypeIndecies.Count > 0) result += $" without: <b> <color=red> {withoutIndeces} </color> </b>";
            return result;
        }
    }
}
