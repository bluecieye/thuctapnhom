import React, { useState, useEffect } from 'react';
import { manufacturerApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Manufacturers = () => {
    const [manufacturers, setManufacturers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingManufacturer, setEditingManufacturer] = useState(null);
    const [formData, setFormData] = useState({ name: '', country: '', description: '', website: '' });
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadManufacturers();
    }, []);

    const loadManufacturers = async () => {
        setLoading(true);
        try {
            const response = await manufacturerApi.getAll();
            setManufacturers(response.data || []);
        } catch (err) {
            console.error('Failed to load manufacturers:', err);
        } finally {
            setLoading(false);
        }
    };

    const openModal = (manufacturer = null) => {
        if (manufacturer) {
            setEditingManufacturer(manufacturer);
            setFormData({
                name: manufacturer.name,
                country: manufacturer.country || '',
                description: manufacturer.description || '',
                website: manufacturer.website || '',
            });
        } else {
            setEditingManufacturer(null);
            setFormData({ name: '', country: '', description: '', website: '' });
        }
        setError('');
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setEditingManufacturer(null);
        setError('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        try {
            if (editingManufacturer) {
                await manufacturerApi.update(editingManufacturer.id, formData);
            } else {
                await manufacturerApi.create(formData);
            }
            closeModal();
            loadManufacturers();
        } catch (err) {
            setError(err.response?.data?.message || 'Operation failed');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Delete this manufacturer? Products linked to it will have no manufacturer.')) return;
        try {
            await manufacturerApi.delete(id);
            loadManufacturers();
        } catch (err) {
            alert('Failed to delete manufacturer');
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Manufacturers Management</h1>
                        </div>
                    </div>
                </div>
            </div>

            <section className="content">
                <div className="container-fluid">
                    <div className="card">
                        <div className="card-header">
                            <div className="d-flex justify-content-between align-items-center">
                                <span className="card-title">All Manufacturers ({manufacturers.length})</span>
                                {isAdmin() && (
                                    <button className="btn btn-success" onClick={() => openModal()}>
                                        <i className="fas fa-plus"></i> Add Manufacturer
                                    </button>
                                )}
                            </div>
                        </div>
                        <div className="card-body">
                            {loading ? (
                                <div className="text-center py-5">
                                    <div className="spinner-border text-primary"></div>
                                </div>
                            ) : (
                                <table className="table table-bordered table-striped">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Name</th>
                                            <th>Country</th>
                                            <th>Description</th>
                                            <th>Website</th>
                                            {isAdmin() && <th>Actions</th>}
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {manufacturers.length === 0 ? (
                                            <tr>
                                                <td colSpan={isAdmin() ? 6 : 5} className="text-center">
                                                    No manufacturers found
                                                </td>
                                            </tr>
                                        ) : (
                                            manufacturers.map(m => (
                                                <tr key={m.id}>
                                                    <td>{m.id}</td>
                                                    <td><strong>{m.name}</strong></td>
                                                    <td>{m.country || '—'}</td>
                                                    <td className="text-truncate" style={{ maxWidth: '300px' }}>{m.description || '—'}</td>
                                                    <td>
                                                        {m.website
                                                            ? <a href={m.website} target="_blank" rel="noreferrer">{m.website}</a>
                                                            : '—'}
                                                    </td>
                                                    {isAdmin() && (
                                                        <td>
                                                            <button className="btn btn-sm btn-info mr-1" onClick={() => openModal(m)}>
                                                                <i className="fas fa-edit"></i>
                                                            </button>
                                                            <button className="btn btn-sm btn-danger" onClick={() => handleDelete(m.id)}>
                                                                <i className="fas fa-trash"></i>
                                                            </button>
                                                        </td>
                                                    )}
                                                </tr>
                                            ))
                                        )}
                                    </tbody>
                                </table>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {showModal && (
                <div className="modal fade show" style={{ display: 'block' }} tabIndex="-1">
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">
                                    {editingManufacturer ? 'Edit Manufacturer' : 'Add Manufacturer'}
                                </h5>
                                <button type="button" className="close" onClick={closeModal}>
                                    <span>&times;</span>
                                </button>
                            </div>
                            <form onSubmit={handleSubmit}>
                                <div className="modal-body">
                                    {error && <div className="alert alert-danger">{error}</div>}
                                    <div className="form-group">
                                        <label>Name *</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={formData.name}
                                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                            required
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Country</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={formData.country}
                                            onChange={(e) => setFormData({ ...formData, country: e.target.value })}
                                            placeholder="e.g. Vietnam, USA, Japan"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Description</label>
                                        <textarea
                                            className="form-control"
                                            value={formData.description}
                                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                            rows="3"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Website</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={formData.website}
                                            onChange={(e) => setFormData({ ...formData, website: e.target.value })}
                                            placeholder="https://..."
                                        />
                                    </div>
                                </div>
                                <div className="modal-footer">
                                    <button type="button" className="btn btn-secondary" onClick={closeModal}>Cancel</button>
                                    <button type="submit" className="btn btn-primary">
                                        {editingManufacturer ? 'Update' : 'Create'}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}
            {showModal && <div className="modal-backdrop fade show"></div>}
        </div>
    );
};

export default Manufacturers;
