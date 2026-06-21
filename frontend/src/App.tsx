import { useEffect, useMemo, useState } from 'react'
import type {
  AgendaItem,
  LogEntry,
  Orcamento,
  Servico,
} from './apiGatewayAdapter'
import { apiGatewayAdapter } from './apiGatewayAdapter'
import './App.css'

const formatCurrency = (value: number) =>
  value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

function App() {
  const [token, setToken] = useState<string>(localStorage.getItem('token') || '')
  const [email, setEmail] = useState('user@example.com')
  const [name, setName] = useState('Usuario Demo')
  const [message, setMessage] = useState('')
  const [servicos, setServicos] = useState<Servico[]>([])
  const [orcamentos, setOrcamentos] = useState<Orcamento[]>([])
  const [agenda, setAgenda] = useState<AgendaItem[]>([])
  const [historico, setHistorico] = useState<LogEntry[]>([])
  const [selectedServicoId, setSelectedServicoId] = useState('')
  const [valorTotal, setValorTotal] = useState('150')
  const [isLoading, setIsLoading] = useState(false)

  const totalValue = useMemo(
    () => servicos.reduce((sum, servico) => sum + servico.valor, 0),
    [servicos],
  )

  const totalBudgets = useMemo(
    () => orcamentos.reduce((sum, item) => sum + item.valorTotal, 0),
    [orcamentos],
  )

  const refreshGatewayData = async (jwt: string) => {
    const [servicosPayload, orcamentosPayload, agendaPayload, historicoPayload] = await Promise.all([
      apiGatewayAdapter.getServicos(jwt),
      apiGatewayAdapter.getOrcamentos(jwt),
      apiGatewayAdapter.getAgenda(jwt),
      apiGatewayAdapter.getHistorico(jwt),
    ])

    setServicos(servicosPayload)
    setOrcamentos(orcamentosPayload)
    setAgenda(agendaPayload)
    setHistorico(historicoPayload)
    setSelectedServicoId((current) => current || servicosPayload[0]?.id || '')
  }

  useEffect(() => {
    if (!token) return

    setIsLoading(true)
    refreshGatewayData(token)
      .catch(() => setMessage('Nao foi possivel carregar os dados do API Gateway.'))
      .finally(() => setIsLoading(false))
  }, [token])

  const handleLogin = async () => {
    setIsLoading(true)
    setMessage('')

    try {
      const authPayload = await apiGatewayAdapter.login(email, name)
      localStorage.setItem('token', authPayload.jwtToken)
      setToken(authPayload.jwtToken)
      await refreshGatewayData(authPayload.jwtToken)
      setMessage(`Bem-vindo(a), ${authPayload.email || email}`)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro inesperado')
    } finally {
      setIsLoading(false)
    }
  }

  const handleSalvarOrcamento = async () => {
    if (!token || !selectedServicoId) return

    setIsLoading(true)
    setMessage('')

    try {
      await apiGatewayAdapter.salvarOrcamento(token, selectedServicoId, Number(valorTotal))
      await refreshGatewayData(token)
      setMessage('Orcamento gravado e evento registrado no historico.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Nao foi possivel gravar o orcamento.')
    } finally {
      setIsLoading(false)
    }
  }

  const handleStatusChange = async (servico: Servico) => {
    if (!token) return

    setIsLoading(true)
    try {
      await apiGatewayAdapter.alterarStatus(token, servico.id, !servico.ativo)
      await refreshGatewayData(token)
      setMessage('Status alterado e evento registrado no historico.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Nao foi possivel alterar o status.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="app-shell">
      <header className="top-bar">
        <div>
          <p className="eyebrow">API Gateway</p>
          <h1>Servicos & Orcamentos</h1>
        </div>
        <section className="login-panel" aria-label="Login do usuario">
          <label>
            E-mail
            <input value={email} onChange={(e) => setEmail(e.target.value)} />
          </label>
          <label>
            Nome
            <input value={name} onChange={(e) => setName(e.target.value)} />
          </label>
          <button onClick={handleLogin} disabled={isLoading}>
            {token ? 'Reautenticar' : 'Entrar'}
          </button>
        </section>
      </header>

      {message && <p className="feedback">{message}</p>}

      <section className="stats-grid">
        <article className="stat-card">
          <span>Servicos ativos</span>
          <strong>{servicos.filter((servico) => servico.ativo).length}</strong>
        </article>
        <article className="stat-card">
          <span>Valor total</span>
          <strong>{formatCurrency(totalValue)}</strong>
        </article>
        <article className="stat-card">
          <span>Orcamentos</span>
          <strong>{orcamentos.length}</strong>
        </article>
        <article className="stat-card">
          <span>Volume mensal</span>
          <strong>{formatCurrency(totalBudgets)}</strong>
        </article>
      </section>

      <section className="content-grid">
        <div className="panel">
          <div className="panel-header">
            <h2>Servicos</h2>
            <button disabled={isLoading || !token} onClick={() => token && refreshGatewayData(token)}>
              Atualizar
            </button>
          </div>
          <ul className="data-list">
            {servicos.length === 0 ? (
              <li>Nenhum servico carregado.</li>
            ) : (
              servicos.map((servico) => (
                <li key={servico.id}>
                  <div>
                    <strong>{servico.nome}</strong>
                    <span>{formatCurrency(servico.valor)}</span>
                  </div>
                  <button onClick={() => handleStatusChange(servico)} disabled={isLoading}>
                    {servico.ativo ? 'Desativar' : 'Ativar'}
                  </button>
                </li>
              ))
            )}
          </ul>
        </div>

        <div className="panel form-panel">
          <h2>Gravar orcamento</h2>
          <label>
            Servico
            <select value={selectedServicoId} onChange={(e) => setSelectedServicoId(e.target.value)}>
              <option value="">Selecione</option>
              {servicos.map((servico) => (
                <option key={servico.id} value={servico.id}>
                  {servico.nome}
                </option>
              ))}
            </select>
          </label>
          <label>
            Valor
            <input value={valorTotal} onChange={(e) => setValorTotal(e.target.value)} inputMode="decimal" />
          </label>
          <button onClick={handleSalvarOrcamento} disabled={isLoading || !selectedServicoId || !token}>
            Salvar
          </button>
        </div>
      </section>

      <section className="content-grid lower-grid">
        <div className="panel">
          <h2>Agenda</h2>
          <ul className="data-list">
            {agenda.length === 0 ? (
              <li>Nenhum item de agenda.</li>
            ) : (
              agenda.map((item) => (
                <li key={item.id}>
                  <div>
                    <strong>{item.titulo}</strong>
                    <span>{new Date(item.data).toLocaleString('pt-BR')}</span>
                  </div>
                  <small>{item.status}</small>
                </li>
              ))
            )}
          </ul>
        </div>

        <div className="panel">
          <h2>Historico de eventos</h2>
          <ul className="data-list history-list">
            {historico.length === 0 ? (
              <li>Nenhum evento registrado.</li>
            ) : (
              historico.map((entry) => (
                <li key={entry.id}>
                  <div>
                    <strong>{entry.eventType}</strong>
                    <span>{new Date(entry.occurredAt).toLocaleString('pt-BR')}</span>
                  </div>
                </li>
              ))
            )}
          </ul>
        </div>
      </section>
    </main>
  )
}

export default App
