using System;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField]
    private int _bulletCount = 0;
    [SerializeField]
    private bool _isAutoExpand = true;
    [SerializeField]
    private Bullet _bulletPrefab = null;
    [SerializeField]
    private CollisionManager _collisionManager = null;
    [SerializeField]
    private ScreenBoundingTrigger _screenBoundingTrigger = null;
    [SerializeField]
    private BulletSpawner _bulletSpawner = null;

    private MonoPoolList<Bullet> _bulletList = null;

    public event EventHandler BulletReleasedEvent = null;
    private void Start()
    {
        _bulletList = new MonoPoolList<Bullet>(_bulletPrefab, this.gameObject, _bulletCount, _isAutoExpand);
        _collisionManager.BulletDestroyedEvent += HandleBulletDestruction;
        _screenBoundingTrigger.BulletOffscreenEvent += HandleBulletDestruction;
        _bulletSpawner.BulletDestroyedEvent += HandleBulletDestruction;

        InitializePooledBullets();
    }

    private void InitializePooledBullets()
    {
        foreach (Bullet bullet in _bulletList.Pool)
        {
            InitializePooledBullet(bullet);
        }
    }

    private void InitializePooledBullet(Bullet instance)
    {
        _collisionManager.Subscribe(instance);
    }

    public Bullet Get()
    {
        if (_bulletList.Get(out Bullet instance))
        {
            return instance;
        }
        InitializePooledBullet(instance);
        return instance;
    }

    public void Release(Bullet bullet)
    {
        bullet.Reset();
        _bulletList.Release(bullet);
    }

    private void HandleBulletDestruction(object sender, BulletEventArgs args)
    {
        Release(args.Bullet);
        BulletReleasedEvent?.Invoke(this, EventArgs.Empty);
    }
}
