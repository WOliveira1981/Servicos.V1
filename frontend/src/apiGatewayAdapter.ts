export type Servico = {
  id: string
  nome: string
  descricao: string
  valor: number
  ativo: boolean
}

export type Orcamento = {
  id: string
  servicoId: string
  valorTotal: number
  dataCriacao: string
  ativo: boolean
}

export type AgendaItem = {
  id: string
  titulo: string
  data: string
  status: string
}

export type LogEntry = {
  id: string
  eventType: string
  payload: string
  occurredAt: string
}

export type LoginResponse = {
  jwtToken: string
  provider: string
  email?: string
}

const API_BASE = process.env.API_BASE_URL || '/api'

const parseJson = async <T>(response: Response): Promise<T> => {
  const payload = await response.json()

  if (!response.ok) {
    throw new Error(payload.message || 'Falha ao chamar o API Gateway')
  }

  return payload as T
}

// Padrao Adapter: este modulo traduz chamadas do React para o contrato HTTP do API Gateway.
// A tela usa metodos de negocio, enquanto headers, rotas e serializacao ficam encapsulados aqui.
export const apiGatewayAdapter = {
  login: async (email: string, name: string): Promise<LoginResponse> => {
    const response = await fetch(`${API_BASE}/gateway/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        provider: 'Google',
        token: 'demo-google-token',
        email,
        name,
      }),
    })

    return parseJson<LoginResponse>(response)
  },

  getServicos: async (token: string): Promise<Servico[]> =>
    parseJson<Servico[]>(
      await fetch(`${API_BASE}/gateway/servicos`, {
        headers: { Authorization: `Bearer ${token}` },
      }),
    ),

  salvarOrcamento: async (token: string, servicoId: string, valorTotal: number): Promise<Orcamento> =>
    parseJson<Orcamento>(
      await fetch(`${API_BASE}/gateway/orcamentos`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ servicoId, valorTotal }),
      }),
    ),

  getOrcamentos: async (token: string): Promise<Orcamento[]> =>
    parseJson<Orcamento[]>(
      await fetch(`${API_BASE}/gateway/orcamentos`, {
        headers: { Authorization: `Bearer ${token}` },
      }),
    ),

  getAgenda: async (token: string): Promise<AgendaItem[]> =>
    parseJson<AgendaItem[]>(
      await fetch(`${API_BASE}/gateway/agenda`, {
        headers: { Authorization: `Bearer ${token}` },
      }),
    ),

  getHistorico: async (token: string): Promise<LogEntry[]> =>
    parseJson<LogEntry[]>(
      await fetch(`${API_BASE}/gateway/historico`, {
        headers: { Authorization: `Bearer ${token}` },
      }),
    ),

  alterarStatus: async (token: string, servicoId: string, ativo: boolean): Promise<Servico> =>
    parseJson<Servico>(
      await fetch(`${API_BASE}/gateway/servicos/${servicoId}/status`, {
        method: 'PATCH',
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ ativo }),
      }),
    ),
}
